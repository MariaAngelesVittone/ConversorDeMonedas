using Dto.Request;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

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
    }
}
