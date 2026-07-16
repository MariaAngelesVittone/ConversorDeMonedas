using System.Collections.Generic;
using Data.Entities;

namespace Data.Interface
{
    public interface IUserRepository
    {
        User? ValidateUsernameLogin(string username);
        User? LoginUser(string username, string password);
        User? FindUser(string username);
        User AddUser(User user);
        User? GetUserById(int userId);
        void Update(User user);
        List<User> GetAllUser();
        User? DeleteUserForAdmin(int id);
        void SaveChanges();
    }
}