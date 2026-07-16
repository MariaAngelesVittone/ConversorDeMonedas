using System.Collections.Generic;
using System.Linq;
using Data.Entities;
using Data.Interface;

namespace Data.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CurrencyConverterContext context) : base(context) { }

        public User? FindUser(string username) => Query().FirstOrDefault(u => u.Username == username);

        public User AddUser(User user)
        {
            Add(user);
            return user;
        }

        public void SaveChanges() => base.SaveChanges();

        public User? ValidateUsernameLogin(string username) => FindUser(username);

        public User? LoginUser(string username, string password) =>
            Query().FirstOrDefault(u => u.Username == username && u.Password == password);

        public User? GetUserById(int userId) => Query().FirstOrDefault(u => u.Id == userId);

        public void Update(User user) => base.Update(user);

        public List<User> GetAllUser() => Query().ToList();

        public User? DeleteUserForAdmin(int id)
        {
            var user = GetUserById(id);
            if (user is null) return null;
            Delete(user);
            return user;
        }
    }
}