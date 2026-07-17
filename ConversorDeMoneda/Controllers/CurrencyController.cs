using Data.Entities;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interface;
using System.Security.Claims;

namespace BackConversorDeMonedasTP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        public CurrencyController(ICurrencyService currencyService, IUserService userService)
        {
            _currencyService = currencyService;
            _userService = userService;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var userId) ? userId : 0;
        }

        [HttpGet]
        public IActionResult GetCurrency()
        {
            List<Currency>? getCurrency = _currencyService.GetCurrency();

            if (getCurrency is null)
            {
                return BadRequest("No hay monedas disponibles");
            }
            return Ok(getCurrency);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateCurrency([FromBody] CreateCurrencyDto createCurrencyDto)
        {
            if (!_userService.IsAdmin(GetCurrentUserId()))
            {
                return Forbid();
            }

            Currency? currency = _currencyService.CreateCurrency(createCurrencyDto);
            if (currency is null)
            {
                return BadRequest($"Ya existe una moneda con el codigo {createCurrencyDto.Codigo}.");
            }
            return Ok(currency);
        }

        [HttpPut("{codeCurrency}")]
        [Authorize]
        public IActionResult ModificateCurrency([FromRoute] int codeCurrency, [FromBody] ModificateCurrencyDto modificateCurrencyDto)
        {
            if (!_userService.IsAdmin(GetCurrentUserId()))
            {
                return Forbid();
            }

            Currency? currency = _currencyService.ModificateCurrency(codeCurrency, modificateCurrencyDto);
            if (currency is null)
            {
                return BadRequest($"El codigo {codeCurrency} no corresponde a ninguna moneda.");
            }
            return Ok(currency);
        }

        [HttpDelete("{codeCurrency}")]
        [Authorize]
        public IActionResult DeleteCurrency([FromRoute] int codeCurrency)
        {
            if (!_userService.IsAdmin(GetCurrentUserId()))
            {
                return Forbid();
            }

            Currency? currency = _currencyService.DeleteCurrency(codeCurrency);
            if (currency is null)
            {
                return BadRequest($"El codigo {codeCurrency} no corresponde a ninguna moneda.");
            }
            else
            {
                return Ok(currency);
            }
        }

        [HttpPost("conversion")]
        [Authorize]
        public IActionResult ConvertCurrency([FromBody] CurrencyConversionDto currencyConversionDto)
        {
            var userId = GetCurrentUserId();

            if (!_userService.CheckConvert(userId))
            {
                return BadRequest("No se pueden realizar mas conversiones. Verifique su plan");
            }

            decimal conversioResult;
            try
            {
                conversioResult = _currencyService.ConvertCurrency(currencyConversionDto, userId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(conversioResult);
        }
    }
}
