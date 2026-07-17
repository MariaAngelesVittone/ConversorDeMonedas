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

        // Copia de la leyenda al momento de convertir, para que el historial
        // siga siendo legible aunque la moneda se borre despues.
        public string FromCurrencyLeyend { get; set; } = string.Empty;

        public string ToCurrencyLeyend { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public decimal Result { get; set; }

        public DateTime Date { get; set; }
    }
}
