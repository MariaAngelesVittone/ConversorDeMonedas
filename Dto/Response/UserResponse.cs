using Data.Entities;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public SubscriptionType Subcription { get; set; }
    }
}
