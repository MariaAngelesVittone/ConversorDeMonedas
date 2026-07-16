using System.Linq;
using System.Security.Claims;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConversorDeMoneda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly CurrencyConverterContext _context;

        public FavoritesController(CurrencyConverterContext context)
        {
            _context = context;
        }

        [HttpPost("{currencyCode:int}")]
        public IActionResult MarkAsFavorite(int currencyCode)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
                return Unauthorized();

            var currency = _context.Currencies.Find(currencyCode);
            if (currency == null) return NotFound("Currency not found.");

            var exists = _context.FavoriteCurrencies
                .Any(f => f.UserId == userId && f.CurrencyCode == currencyCode);

            if (exists) return BadRequest("Already in favorites.");

            var favorite = new FavoriteCurrency
            {
                UserId = userId,
                CurrencyCode = currencyCode
            };

            _context.FavoriteCurrencies.Add(favorite);
            _context.SaveChanges();

            return Ok(new { message = "Added to favorites successfully." });
        }

        [HttpDelete("{currencyCode:int}")]
        public IActionResult RemoveFavorite(int currencyCode)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
                return Unauthorized();

            var favorite = _context.FavoriteCurrencies
                .FirstOrDefault(f => f.UserId == userId && f.CurrencyCode == currencyCode);

            if (favorite == null) return NotFound("Not in favorites.");

            _context.FavoriteCurrencies.Remove(favorite);
            _context.SaveChanges();

            return Ok(new { message = "Removed from favorites." });
        }

        [HttpGet("all-with-favorites")]
        public IActionResult GetAllWithFavorites()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
                return Unauthorized();

            var favoriteCodes = _context.FavoriteCurrencies
                .Where(f => f.UserId == userId)
                .Select(f => f.CurrencyCode)
                .ToList();

            var allCurrencies = _context.Currencies
                .Select(c => new
                {
                    c.Code,
                    c.Leyend,
                    c.Symbol,
                    c.ConversionRate,
                    IsFavorite = favoriteCodes.Contains(c.Code)
                })
                .OrderByDescending(c => c.IsFavorite)
                .ThenBy(c => c.Code)
                .ToList();

            return Ok(allCurrencies);
        }
    }
}
