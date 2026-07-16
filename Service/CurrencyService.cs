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

        public List<Currency>? GetCurrency()
        {
            var lista = _currencyRepo.GetAll();
            return lista != null && lista.Any() ? lista : null;
        }

        public Currency? CreateCurrency(CreateCurrencyDto dto)
        {
            if (_currencyRepo.GetByCode(dto.Codigo) is not null)
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

            var created = _currencyRepo.Create(currency);
            _currencyRepo.SaveChanges();
            return created;
        }

        public Currency? ModificateCurrency(int codeCurrency, ModificateCurrencyDto dto)
        {
            var currency = _currencyRepo.GetByCode(codeCurrency);
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

            var updated = _currencyRepo.Update(currency);
            _currencyRepo.SaveChanges();
            return updated;
        }

        public Currency? DeleteCurrency(int codeCurrency)
        {
            var deleted = _currencyRepo.Delete(codeCurrency);
            if (deleted is not null) _currencyRepo.SaveChanges();
            return deleted;
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
            var from = _currencyRepo.GetByCode(dto.FromCurrencyCode)
                ?? throw new ArgumentException("Moneda origen desconocida.");
            var to = _currencyRepo.GetByCode(dto.ToCurrencyCode)
                ?? throw new ArgumentException("Moneda destino desconocida.");

            if (to.ConversionRate <= 0)
                throw new ArgumentException("Índice de convertibilidad inválido para la moneda destino.");

            if (from.Code == to.Code) return dto.Amount;
            return dto.Amount * from.ConversionRate / to.ConversionRate;
        }
    }
}