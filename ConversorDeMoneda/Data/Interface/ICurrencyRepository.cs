using System.Collections.Generic;
using Data.Entities;

namespace Data.Interface
{
    public interface ICurrencyRepository
    {
        List<Currency> GetAll();
        Currency Create(Currency currency);
        Currency? GetByCode(int code);
        Currency Update(Currency currency);
        Currency? Delete(int code);
        void SaveChanges();
    }
}
