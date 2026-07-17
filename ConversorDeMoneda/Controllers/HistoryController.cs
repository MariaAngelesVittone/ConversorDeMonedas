using System;
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

            // Sqlite no conserva el DateTimeKind al leer; lo forzamos a Utc
            // para que el JSON viaje con la "Z" y el frontend lo convierta bien.
            foreach (var item in history)
            {
                item.Date = DateTime.SpecifyKind(item.Date, DateTimeKind.Utc);
            }

            return Ok(history);
        }
    }
}
