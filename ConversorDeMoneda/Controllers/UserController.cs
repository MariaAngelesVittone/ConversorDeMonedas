using Data.Enums;
using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Security.Claims;

namespace ConversorDeMoneda.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromBody] UserRegisterDTO userRegisterDTO)
        {
            try
            {
                var userResponse = _userService.UserRegistered(userRegisterDTO);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var userId) ? userId : 0;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var profile = _userService.GetProfile(userId);
            if (profile is null) return NotFound();

            return Ok(profile);
        }

        [HttpPut("subscription")]
        [Authorize]
        public IActionResult UpdateMySubscription([FromBody] SubscriptionType newType)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var profile = _userService.UpdateOwnSubscription(userId, newType);
            if (profile is null) return NotFound();

            return Ok(profile);
        }
    }
}
