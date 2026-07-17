using System;
using System.Collections.Generic;
using System.Linq;
using Data.Entities;
using Data.Interface;

namespace Data.Repository
{
    public class ConversionHistoryRepository : BaseRepository<ConversionHistory>, IConversionHistoryRepository
    {
        public ConversionHistoryRepository(CurrencyConverterContext context) : base(context) { }

        public int CountSince(int userId, DateTime since) =>
            Query().Count(h => h.UserId == userId && h.Date >= since);

        public List<ConversionHistory> GetByUserId(int userId) =>
            Query().Where(h => h.UserId == userId).OrderByDescending(h => h.Date).ToList();

        public void Add(ConversionHistory history) => base.Add(history);
    }
}
