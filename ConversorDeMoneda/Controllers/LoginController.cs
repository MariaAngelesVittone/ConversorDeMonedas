using Data.Entities;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service;
using Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackConversorDeMonedasTP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public LoginController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult LoginUser(LoginUserDto loginUserDto)
        {
            User? validateLoginUser = _userService.LoginUser(loginUserDto.Username, loginUserDto.Password);

            if (validateLoginUser is null)
            {
                return BadRequest("Credenciales incorrectas, intentelo de nuevo");
            }

            if (validateLoginUser is not null)
            {
                var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));

                var credentials = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);

                var claimsForToken = new List<Claim>();
                claimsForToken.Add(new Claim("sub", validateLoginUser.Id.ToString()));
                claimsForToken.Add(new Claim("given_name", validateLoginUser.Username));
                claimsForToken.Add(new Claim("Rol", validateLoginUser.IsAdmin.ToString()));

                var jwtSecurityToken = new JwtSecurityToken(
                  _configuration["Authentication:Issuer"],
                  _configuration["Authentication:Audience"],
                  claimsForToken,
                  DateTime.UtcNow,
                  DateTime.UtcNow.AddHours(1),
                  credentials);

                var tokenToReturn = new JwtSecurityTokenHandler()
                    .WriteToken(jwtSecurityToken);

                return Ok(tokenToReturn);
            }
            return Unauthorized();
        }
    }
}