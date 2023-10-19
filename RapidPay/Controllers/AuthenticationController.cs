using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RapidPay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        
        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        private class RapidPayUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Zip { get; set; }

            public RapidPayUser(
                int userId,
                string userName,
                string firstName,
                string lastName,
                string zip)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                Zip = zip;
            }

        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(
            AuthenticationRequestBody authenticationRequestBody)
        {
            
            var user = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);

            if (user == null)
                return Unauthorized();

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("first_name", user.FirstName));
            claimsForToken.Add(new Claim("last_name", user.LastName));
            claimsForToken.Add(new Claim("zip", user.Zip));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private RapidPayUser ValidateUserCredentials(string? userName, string? password)
        {

            //real authentication would need to be inserted here, hardcoding one working user for demo
            if (userName == "user" && password == "password")
                return new RapidPayUser(1, userName ?? "", "John", "Doe", "90210");
            else
                return null;


        }


    }
}
