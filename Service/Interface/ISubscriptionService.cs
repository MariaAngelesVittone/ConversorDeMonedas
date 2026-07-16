using Data.Entities;

namespace Service.Interface
{
    public interface ISubscriptionService
    {
        Subscription? GetByUserId(int userId);
        bool CanUserConvert(int userId);
        void UpdateSubscription(Subscription updated);
    }
}
