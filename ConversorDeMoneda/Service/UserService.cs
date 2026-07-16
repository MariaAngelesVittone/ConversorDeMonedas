using Data.Entities;
using Data.Enums;
using Data.Interface;
using Dto.Request;
using Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService
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

            return new UserResponse
            {
                Id = userResponse.Id,
                Username = userResponse.Username,
                Subcription = userResponse.SubscriptionType
            };
        }
    }
}
