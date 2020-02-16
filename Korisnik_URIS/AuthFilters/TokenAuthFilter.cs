using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace Korisnik_URIS.AuthFilters
{
    public class TokenAuthFilter : Attribute, IAuthenticationFilter
    {
        public const string SupportedTokenScheme = "Bearer";
        public bool AllowMultiple { get { return false; } }

        private readonly string audience = "_audience";
        private readonly string issuer = "_issuer";

        public bool SendChallenge { get; set; }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var authHeader = context.Request.Headers.Authorization;
            if (authHeader == null)
                return;
            var tokenType = authHeader.Scheme;
            if (!tokenType.Equals(SupportedTokenScheme))
                return;
            var credentials = authHeader.Parameter;
            if (String.IsNullOrEmpty(credentials))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", context.Request);
                return;
            }
            try
            {
                IPrincipal principal = await ValidateCredentialsAsync(credentials, context.Request, cancellationToken);
                if (principal == null)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Invalid security token", context.Request);
                }
                else
                {
                    // We have a valid, authenticated user; save off the IPrincipal instance
                    context.Principal = principal;
                }
            }
            catch (Exception stex) when (stex is SecurityTokenInvalidLifetimeException ||
                                             stex is SecurityTokenExpiredException ||
                                             stex is SecurityTokenNoExpirationException ||
                                             stex is SecurityTokenNotYetValidException)
            {
                context.ErrorResult = new AuthenticationFailureResult("Security token lifetime error", context.Request);
            }
            catch (Exception)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid security token", context.Request);
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            if (SendChallenge)
            {
                context.Result = new ChallengeOnUnauthorizedResult(
                    new AuthenticationHeaderValue(SupportedTokenScheme),
                    context.Result);
            }

            return Task.FromResult(0);
        }
        private async Task<IPrincipal> ValidateCredentialsAsync(string credentials,
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            // da li je token
            var isValidJwt = jwtHandler.CanReadToken(credentials);
            if (!isValidJwt)
                return null;
            var signingKey = new SymmetricSecurityKey((Encoding.ASCII.GetBytes("secret key secret key secret key secret key secret key secret keey")));


            //var key = "this is the secret key to add some default jwt token, lets see how it works";
            //var certificate = new X509Certificate2(key);
            //var signingKey = new X509SecurityKey(certificate);

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudiences = new[] { audience },

                ValidateIssuer = true,
                ValidIssuers = new[] { issuer },

                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = new[] { signingKey },

                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(5),  

                NameClaimType = ClaimTypes.NameIdentifier,
                AuthenticationType = SupportedTokenScheme
            };

            SecurityToken validatedToken = new JwtSecurityToken();
            ClaimsPrincipal principal = jwtHandler.ValidateToken(credentials, validationParameters, out validatedToken);

            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("urn:Issuer",
                validatedToken.Issuer));
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("urn:TokenScheme",
                SupportedTokenScheme));
            //down - stream
            ((ClaimsIdentity)principal.Identity).BootstrapContext = credentials;

            return await Task.FromResult(principal);
        }
    }
}