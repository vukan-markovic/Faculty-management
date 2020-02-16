using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace DepartmanURIS.AuthFilters
{
    public class BasicAuthFilterAttribute : Attribute, IAuthenticationFilter
    {
        public const string SupportedTokenScheme = "Basic";
        public bool AllowMultiple { get { return false; } }
        public bool SendChallenge { get; set; }
        public BasicAuthFilterAttribute()
        {
            SendChallenge = true;
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context,
            CancellationToken cancellationToken)
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
            IPrincipal principal = await ValidateCredentialsAsync(credentials, cancellationToken);
            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", context.Request);
            }
            else
            {
                context.Principal = principal;
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
        private async Task<IPrincipal> ValidateCredentialsAsync(string credentials, CancellationToken cancellationToken)
        {
            var subject = ParseBasicAuthCredential(credentials);

            if (string.IsNullOrEmpty(subject.Item1)||subject.Item1!="admin" || string.IsNullOrEmpty(subject.Item2) || subject.Item2 != "admin")
                return null;

            IList<Claim> claimCollection = new List<Claim>
            {
                new Claim(ClaimTypes.Name, subject.Item1),
                new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToString("o")),
                new Claim(ClaimTypes.Role,"Admin")
            };
            var identity = new ClaimsIdentity(claimCollection, SupportedTokenScheme);
            var principal = new ClaimsPrincipal(identity);

            return await Task.FromResult(principal);
        }

        private Tuple<string, string> ParseBasicAuthCredential(string credential)
        {
            string password = null;
            var subject = (Encoding.GetEncoding("iso-8859-1").GetString(Convert.FromBase64String(credential)));
            if (string.IsNullOrEmpty(subject))
                return new Tuple<string, string>(null, null);

            if (subject.Contains(":"))
            {
                var index = subject.IndexOf(':');
                password = subject.Substring(index + 1);
                subject = subject.Substring(0, index);
            }

            return new Tuple<string, string>(subject, password);
        }
    }
}