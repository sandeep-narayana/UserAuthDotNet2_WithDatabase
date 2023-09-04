using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserAuthDotBet2_WithDatabase.Repositories;

namespace UserAuthDotBet2_WithDatabase
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private IConfiguration _config;
        private IAuthRepository _auth;

        public AuthController(ILogger<AuthController> logger, IConfiguration config, IAuthRepository auth)
        {
            _logger = logger;
            _config = config;
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] UserCredentials userCredentials)
        {

            try
            {
                // Validate the registration request, check if the username or email is already taken, etc.
                // Perform registration logic, such as creating a new user in your database with hashed password.

                // For this example, let's assume you have a method in your _auth repository to handle registration.
                var registrationResult = await _auth.RegisterUser(userCredentials);

                if (registrationResult)
                {
                    // Registration successful, generate and return a JWT token.
                    var token = GenerateToken(userCredentials.Username);
                    return Ok(token);
                }
                else
                {
                    // Registration failed, return an appropriate response.
                    return BadRequest("Registration failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return StatusCode(500, "Internal Server Error");
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserCredentials userCredentials)
        {
            try
            {
                var didAuthorize = await _auth.CheckAuthentication(userCredentials);

                if (!didAuthorize)
                {
                    // Unauthorized, return a 401 response.
                    return Unauthorized("Authentication failed");
                }

                // Authentication successful, generate and return a JWT token.
                var token = GenerateToken(userCredentials.Username);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Welcome")]
        [Authorize]
        public async Task<ActionResult<string>> Welcome()
        {
            try
            {
                // Authorized user, return a welcome message.
                return Ok("Welcome, you are an authenticated user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the welcome request.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private string GenerateToken(string name)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            const string Name = nameof(Name);

            var claims = new[]
            {
            new Claim(Name, name),
        };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60 * 48),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class UserCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

    }
}