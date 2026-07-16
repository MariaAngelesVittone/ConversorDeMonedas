using System;
using System.Linq;
using Data;
using Data.Interface;
using Data.Entities;
using Data.Enums;

namespace Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly CurrencyConverterContext _context;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, CurrencyConverterContext context)
        {
            _subscriptionRepository = subscriptionRepository;
            _context = context;
        }

        public Subscription? GetByUserId(int userId) =>
            _subscriptionRepository.GetByUserId(userId);

        public bool CanUserConvert(int userId)
        {
            var subscription = _subscriptionRepository.GetByUserId(userId);
            if (subscription == null || !subscription.IsActive) return false;

            int allowed = subscription.Type switch
            {
                SubscriptionType.Free => 10,
                SubscriptionType.Trial => 100,
                SubscriptionType.Pro => int.MaxValue,
                _ => 0
            };

            var since = DateTime.UtcNow.AddDays(-30);
            var used = _context.ConversionHistories.Count(h => h.UserId == userId && h.Date >= since);

            return used < allowed;
        }

        public void UpdateSubscription(Subscription updated)
        {
            _subscriptionRepository.Update(updated);
            _subscriptionRepository.SaveChanges();
        }
    }
}