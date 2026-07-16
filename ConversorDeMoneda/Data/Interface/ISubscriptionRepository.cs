using Data.Entities;

namespace Data.Interface
{
    public interface ISubscriptionRepository
    {
        Subscription? GetByUserId(int userId);
        void Update(Subscription subscription);
    }
}