using Data.Entities;
using Data.Interface;

namespace Data.Repository
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(CurrencyConverterContext context) : base(context) { }

        public Subscription? GetByUserId(int userId) =>
            Query().FirstOrDefault(s => s.UserId == userId); 

        public void Update(Subscription subscription) => base.Update(subscription);
    }
}