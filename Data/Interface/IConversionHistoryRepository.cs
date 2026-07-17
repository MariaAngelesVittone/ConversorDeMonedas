using System;
using System.Collections.Generic;
using Data.Entities;

namespace Data.Interface
{
    public interface IConversionHistoryRepository
    {
        int CountSince(int userId, DateTime since);
        List<ConversionHistory> GetByUserId(int userId);
        void Add(ConversionHistory history);
        void SaveChanges();
    }
}
