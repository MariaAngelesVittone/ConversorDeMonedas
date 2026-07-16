using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class ConversionHistory
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int FromCurrencyCode { get; set; }

        public int ToCurrencyCode { get; set; }

        public decimal Amount { get; set; }

        public decimal Result { get; set; }

        public DateTime Date { get; set; }
    }
}
