using System.Collections.Generic;
using Data.Entities;
using Dto;

namespace Service.Interface
{
    public interface ICurrencyService
    {
        List<Currency>? GetCurrency();
        Currency? CreateCurrency(CreateCurrencyDto dto);
        Currency? ModificateCurrency(int codeCurrency, ModificateCurrencyDto dto);
        Currency? DeleteCurrency(int codeCurrency);
        int GetUserIdFromToken(string token);
        decimal ConvertCurrency(CurrencyConversionDto dto, int userId);
    }
}
