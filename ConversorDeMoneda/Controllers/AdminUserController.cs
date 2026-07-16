using System;
using System.Linq;
using System.Security.Claims;
using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConversorDeMoneda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminUserController : ControllerBase
    {
        private readonly CurrencyConverterContext _context;
            
        public AdminUserController(CurrencyConverterContext context)
        {
            _context = context;
        }

        private bool IsCurrentUserAdmin()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out var userId))
                return false;

            var user = _context.Users.Find(userId);
            return user != null && user.IsAdmin;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            if (!IsCurrentUserAdmin()) return Forbid("Admin rights required.");

            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Username,
                    u.SubscriptionType,
                    u.ConversionLimit,
                    u.IsAdmin
                })
                .ToList();

            return Ok(users);
        }

        [HttpPut("{id}/subscription")]
        public IActionResult UpdateSubscription(int id, [FromBody] SubscriptionType newType)
        {
            if (!IsCurrentUserAdmin()) return Forbid("Admin rights required.");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound("User not found.");

            user.SubscriptionType = newType;

            user.ConversionLimit = newType switch
            {
                SubscriptionType.Free => 10,
                SubscriptionType.Trial => 100,
                SubscriptionType.Pro => int.MaxValue,
                _ => user.ConversionLimit
            };

            var subscription = _context.Subscriptions.FirstOrDefault(s => s.UserId == id);
            if (subscription == null)
            {
                subscription = new Subscription
                {
                    UserId = id,
                    Type = newType,
                    StartDate = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Subscriptions.Add(subscription);
            }
            else
            {
                subscription.Type = newType;
                subscription.StartDate = DateTime.UtcNow;
                subscription.IsActive = true;
                _context.Subscriptions.Update(subscription);
            }

            _context.SaveChanges();
            return Ok(new { message = "User subscription updated successfully.", user.SubscriptionType, user.ConversionLimit });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (!IsCurrentUserAdmin()) return Forbid("Admin rights required.");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound("User not found.");

            var favorites = _context.FavoriteCurrencies.Where(f => f.UserId == id).ToList();
            if (favorites.Any())
            {
                _context.FavoriteCurrencies.RemoveRange(favorites);
            }

            var histories = _context.ConversionHistories.Where(h => h.UserId == id).ToList();
            if (histories.Any())
            {
                _context.ConversionHistories.RemoveRange(histories);
            }

            var subscriptions = _context.Subscriptions.Where(s => s.UserId == id).ToList();
            if (subscriptions.Any())
            {
                _context.Subscriptions.RemoveRange(subscriptions);
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
