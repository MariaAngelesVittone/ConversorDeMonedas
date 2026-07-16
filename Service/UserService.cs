using Data.Entities;
using Data.Enums;
using Data.Interface;
using Dto.Request;
using Dto.Response;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserResponse UserRegistered(UserRegisterDTO userRegisterDTO)
        {
            User? existingUser = _userRepository.FindUser(userRegisterDTO.Username);

            if ( existingUser is not null)
            {
                throw new("This username already exists.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDTO.Password);

            User userResponse = new User
            {
                Email = userRegisterDTO.Email,
                Username = userRegisterDTO.Username,
                Password = passwordHash,
                SubscriptionType = SubscriptionType.Free,
                ConversionLimit = 10,
                ConversionUsed = 0,
            };

            _userRepository.AddUser(userResponse);
            _userRepository.SaveChanges();

            return new UserResponse
            {
                Id = userResponse.Id,
                Username = userResponse.Username,
                Subcription = userResponse.SubscriptionType
            };
        }

        public User? LoginUser(string username, string password)
        {
            var user = _userRepository.FindUser(username);
            if (user is null) return null;

            return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
        }

        public bool CheckConvert(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            if (user is null) return false;

            return user.ConversionUsed < user.ConversionLimit;
        }

        public void RegisterConversion(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            if (user is null) return;

            user.ConversionUsed++;
            _userRepository.Update(user);
            _userRepository.SaveChanges();
        }
    }
}
