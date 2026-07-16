using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class FavoriteCurrency
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CurrencyCode { get; set; }
    }
}
