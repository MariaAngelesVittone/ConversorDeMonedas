using Data.Entities;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interface;

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

        private int GetUserIdFromRequest()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return 0;
            }

            var token = authHeader.Substring("Bearer ".Length);
            return _currencyService.GetUserIdFromToken(token);
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
        public IActionResult CreateCurrency([FromBody] CreateCurrencyDto createCurrencyDto)
        {
            var userId = GetUserIdFromRequest();
            if (userId == 0)
            {
                return Unauthorized("Token no proporcionado o invalido.");
            }
            if (!_userService.IsAdmin(userId))
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
        public IActionResult ModificateCurrency([FromRoute] int codeCurrency, [FromBody] ModificateCurrencyDto modificateCurrencyDto)
        {
            var userId = GetUserIdFromRequest();
            if (userId == 0)
            {
                return Unauthorized("Token no proporcionado o invalido.");
            }
            if (!_userService.IsAdmin(userId))
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
        public IActionResult DeleteCurrency([FromRoute] int codeCurrency)
        {
            var userId = GetUserIdFromRequest();
            if (userId == 0)
            {
                return Unauthorized("Token no proporcionado o invalido.");
            }
            if (!_userService.IsAdmin(userId))
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
        public IActionResult ConvertCurrency([FromBody] CurrencyConversionDto currencyConversionDto)
        {
            var userId = GetUserIdFromRequest();

            if (userId == 0)
            {
                return Unauthorized("Token inválido o UserId no encontrado.");
            }
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
