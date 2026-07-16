using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }   
        public string Username { get; set; }
        public string Password { get; set; }

        public int ConversionLimit { get; set; }

        public int ConversionUsed { get; set; }

        public SubscriptionType SubscriptionType { get; set; }

        public bool IsAdmin { get; set; } = false;

        public UserStateForAdmin stateForAdmin { get; set; }

    }
}
