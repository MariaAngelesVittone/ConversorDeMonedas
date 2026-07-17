using Data.Enums;

namespace Dto
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public SubscriptionType SubscriptionType { get; set; }
        public int ConversionLimit { get; set; }
        public bool IsAdmin { get; set; }
    }
}
