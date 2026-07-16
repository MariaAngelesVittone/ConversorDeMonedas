using System.Collections.Generic;
using System.Linq;
using Data.Entities;
using Data.Interface;

namespace Data.Repository
{
    public class CurrencyRepository : BaseRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(CurrencyConverterContext context) : base(context) { }

        public List<Currency> GetAll() => Query().ToList();

        public Currency Create(Currency currency)
        {
            Add(currency);
            return currency;
        }

        public Currency? GetByCode(int code) => Query().FirstOrDefault(c => c.Code == code);

        public Currency Update(Currency currency)
        {
            base.Update(currency);
            return currency;
        }

        public Currency? Delete(int code)
        {
            var entity = GetByCode(code);
            if (entity is null) return null;
            Delete(entity);
            return entity;
        }
    }
}