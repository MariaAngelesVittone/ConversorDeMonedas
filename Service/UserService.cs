using Data.Entities;
using Data.Enums;
using Data.Interface;
using Dto;
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
        private readonly IConversionHistoryRepository _historyRepository;

        public UserService(IUserRepository userRepository, IConversionHistoryRepository historyRepository)
        {
            _userRepository = userRepository;
            _historyRepository = historyRepository;
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

            if (user.SubscriptionType == SubscriptionType.Pro) return true;

            var since = DateTime.UtcNow.AddDays(-30);
            var usedThisMonth = _historyRepository.CountSince(userId, since);
            return usedThisMonth < user.ConversionLimit;
        }

        public bool IsAdmin(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            return user?.IsAdmin ?? false;
        }

        public UserProfileDto? GetProfile(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            if (user is null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                SubscriptionType = user.SubscriptionType,
                ConversionLimit = user.ConversionLimit,
                IsAdmin = user.IsAdmin,
                CanConvert = CheckConvert(userId)
            };
        }

        public UserProfileDto? UpdateOwnSubscription(int userId, SubscriptionType newType)
        {
            var user = _userRepository.GetUserById(userId);
            if (user is null) return null;

            user.SubscriptionType = newType;
            user.ConversionLimit = newType switch
            {
                SubscriptionType.Free => 10,
                SubscriptionType.Trial => 100,
                SubscriptionType.Pro => int.MaxValue,
                _ => user.ConversionLimit
            };

            _userRepository.Update(user);
            _userRepository.SaveChanges();

            return GetProfile(userId);
        }
    }
}
