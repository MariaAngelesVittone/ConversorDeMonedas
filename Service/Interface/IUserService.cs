using Data.Entities;
using Data.Enums;
using Dto;
using Dto.Request;
using Dto.Response;

namespace Service.Interface
{
    public interface IUserService
    {
        UserResponse UserRegistered(UserRegisterDTO userRegisterDTO);
        User? LoginUser(string username, string password);
        bool CheckConvert(int userId);
        bool IsAdmin(int userId);
        UserProfileDto? GetProfile(int userId);
        UserProfileDto? UpdateOwnSubscription(int userId, SubscriptionType newType);
    }
}
