using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IConversionHistoryRepository _historyRepo;

        public CurrencyService(ICurrencyRepository currencyRepo, IUserService userService, IConversionHistoryRepository historyRepo)
        {
            _currencyRepo = currencyRepo;
            _userService = userService;
            _historyRepo = historyRepo;
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
                "BRL" => "R$",
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
                "BRL" => "R$",
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

        public decimal ConvertCurrency(CurrencyConversionDto dto, int userId)
        {
            var from = _currencyRepo.GetByCode(dto.FromCurrencyCode)
                ?? throw new ArgumentException("Moneda origen desconocida.");
            var to = _currencyRepo.GetByCode(dto.ToCurrencyCode)
                ?? throw new ArgumentException("Moneda destino desconocida.");

            if (to.ConversionRate <= 0)
                throw new ArgumentException("Índice de convertibilidad inválido para la moneda destino.");

            var result = from.Code == to.Code ? dto.Amount : dto.Amount * from.ConversionRate / to.ConversionRate;

            _historyRepo.Add(new ConversionHistory
            {
                UserId = userId,
                FromCurrencyCode = dto.FromCurrencyCode,
                ToCurrencyCode = dto.ToCurrencyCode,
                FromCurrencyLeyend = from.Leyend,
                ToCurrencyLeyend = to.Leyend,
                Amount = dto.Amount,
                Result = result,
                Date = DateTime.UtcNow
            });
            _historyRepo.SaveChanges();

            return result;
        }
    }
}