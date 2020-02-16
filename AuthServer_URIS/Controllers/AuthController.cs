using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AuthServer_URIS.Certificat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer_URIS.Controllers
{
    [Route("api-auth/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public AuthController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            User user = new User();
            string connectionString = Configuration["ConnectionStrings:ConnectionString"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql ="SELECT * FROM Korisnik.Korisnik WHERE Email=@Email AND Sifra=@Password";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("Email", email);
                    command.Parameters.AddWithValue("Password", password);
                    try {
                        connection.Open();
                        using (var dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                user.Email = Convert.ToString(dataReader["Email"]);
                                user.Ime = Convert.ToString(dataReader["Ime"]);
                                user.Prezime = Convert.ToString(dataReader["Prezime"]);
                                user.TrenutnaUloga = Convert.ToString(dataReader["TrenutnaUloga"]);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                    
                }
            }
            if (user.Email == null)
                return Unauthorized("Unauthorized");
            var time = DateTime.UtcNow;
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, time.ToUniversalTime().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Role,user.TrenutnaUloga)
            };

            //var filePath = Path.GetFileName("~/Certificat/UrisCert.cer");
            //var certificate = new X509Certificate2(filePath);
            //var _signingKey = new X509SecurityKey(certificate);
            var signingKey = new SymmetricSecurityKey((Encoding.ASCII.GetBytes(Key.KEY)));
            
            var jwt = new JwtSecurityToken(
                    issuer: "_issuer",
                    audience: "_audience",
                    claims: claims,
                    notBefore:time,
                    expires: time.Add(TimeSpan.FromMinutes(60)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var responseJson = new
            {
                access_token = encodedJwt,
                expires_in_seconds = (int)TimeSpan.FromMinutes(60).TotalSeconds
            };
            return Ok(responseJson);
        }
    }

    public class User
    {
        public string Ime;
        public string Prezime;
        public string Email;
        public string TrenutnaUloga;
    }
}