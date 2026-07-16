using Data.Entities;
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
        void RegisterConversion(int userId);
    }
}
