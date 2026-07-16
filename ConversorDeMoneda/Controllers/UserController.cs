using Dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace ConversorDeMoneda.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class UserController : Controller
    {
        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromBody] UserRegisterDTO userRegisterDTO)
        {
            return Ok();
        }
    }
}
