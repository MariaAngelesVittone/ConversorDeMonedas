using System.Linq;
using System.Security.Claims;
using Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConversorDeMoneda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly IConversionHistoryRepository _historyRepository;

        public HistoryController(IConversionHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        [HttpGet]
        public IActionResult GetMyHistory()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
            {
                return Unauthorized();
            }

            var history = _historyRepository.GetByUserId(userId);
            return Ok(history);
        }
    }
}
