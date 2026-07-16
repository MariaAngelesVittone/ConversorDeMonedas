using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Interface;
using Data.Entities;
using Data.Enums;
using Service.Interface;
using Dto; 

namespace Service
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepo;
        private readonly IUserService _userService;

        public CurrencyService(ICurrencyRepository currencyRepo, IUserService userService)
        {
            _currencyRepo = currencyRepo;
            _userService = userService;
        }

        private readonly Dictionary<string, decimal> _conversionIndexes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ARS", 0.002m },
            { "EUR", 1.09m },
            { "KC" , 0.043m },
            { "USD" , 1.00m }
        };

        public List<Currency>? GetCurrency()
        {
            var lista = _currencyRepo.GetCurrency();
            return lista != null && lista.Any() ? lista : null;
        }

        public Currency? CreateCurrency(CreateCurrencyDto dto)
        {
            if (_currencyRepo.VerificateCodeCurrency(dto.Codigo) is not null)
                return null;

            var simbolo = dto.Leyenda switch
            {
                "ARS" => "$",
                "EUR" => "€",
                "CORONA CHECA" => "Kč",
                "USD" => "$",
                _ => dto.Simbolo ?? string.Empty
            };

            var currency = new Currency
            {
                Code = dto.Codigo,
                Leyend = dto.Leyenda,
                Symbol = simbolo,
                ConversionRate = dto.idc,
                State = CurrencyState.Active
            };

            return _currencyRepo.CreateCurrency(currency);
        }

        public Currency? ModificateCurrency(int codeCurrency, ModificateCurrencyDto dto)
        {
            var currency = _currencyRepo.VerificateCodeCurrency(codeCurrency);
            if (currency is null) return null;

            currency.Leyend = dto.Leyenda;
            currency.ConversionRate = dto.idc;
            currency.State = CurrencyState.Modified;
            currency.Symbol = dto.Leyenda switch
            {
                "ARS" => "$",
                "EUR" => "€",
                "CORONA CHECA" => "Kč",
                "USD" => "$",
                _ => currency.Symbol
            };

            return _currencyRepo.ModificateCurrency(currency);
        }

        public Currency? DeleteCurrency(int codeCurrency)
        {
            return _currencyRepo.DeleteCurrency(codeCurrency);
        }

        // Soporta tanto "sub" como ClaimTypes.NameIdentifier / "nameid"
        public int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (string.IsNullOrEmpty(token) || !handler.CanReadToken(token)) return 0;

            try
            {
                var jwt = handler.ReadJwtToken(token);
                var userIdClaim = jwt.Claims.FirstOrDefault(c =>
                    c.Type == "sub" ||
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "nameid")?.Value;

                return int.TryParse(userIdClaim, out var userId) ? userId : 0;
            }
            catch
            {
                return 0;
            }
        }

        public decimal ConvertCurrency(CurrencyConversionDto dto)
        {
            if (!_conversionIndexes.TryGetValue(dto.FromCurrency, out var fromIndex))
                throw new ArgumentException("Moneda origen desconocida.");
            if (!_conversionIndexes.TryGetValue(dto.ToCurrency, out var toIndex))
                throw new ArgumentException("Moneda destino desconocida.");

            if (fromIndex == toIndex) return dto.Amount;
            return dto.Amount * (toIndex / fromIndex);
        }
    }
}