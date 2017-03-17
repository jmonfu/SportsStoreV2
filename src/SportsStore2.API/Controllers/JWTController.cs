using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;
using SportsStore2.API.Options;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SportsStore2.API.Controllers
{
    [Produces("application/json")]
    [Route("api/JWT")]
    public class JwtController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private static SignInManager<ApplicationUser> _signInManager;


        public JwtController(
            IOptions<JwtIssuerOptions> jwtOptions, 
            ILoggerFactory loggerFactory, 
            SignInManager<ApplicationUser> signInManager
            )
        {
            _signInManager = signInManager;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = loggerFactory.CreateLogger<JwtController>();

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] LoginViewModel loginViewModel)
        {
            var identity = await GetClaimsIdentity(loginViewModel);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid username ({loginViewModel.Email}) or password ({loginViewModel.Password})");
                return BadRequest("Invalid credentials");
            }

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, loginViewModel.Email),
            new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
            new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
            identity.FindFirst("UserAuthorized")
          };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private Task<ClaimsIdentity> GetClaimsIdentity(LoginViewModel user)
        {
            if (GetResult(user).Result.Succeeded)
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.Email, "Token"),
                  new[]
                  {
                new Claim("UserAuthorized", "True")
                  }));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }

        private async Task<SignInResult> GetResult(LoginViewModel userModel)
        {
            SignInResult result;
            try
            {
                result = await _signInManager.PasswordSignInAsync(userModel.Email,
                    userModel.Password,
                    userModel.RememberMe,
                    lockoutOnFailure: false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return result;
        }
    }

}