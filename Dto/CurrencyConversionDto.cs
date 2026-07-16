namespace Dto
{
    public class CurrencyConversionDto
    {
        public int FromCurrencyCode { get; set; }
        public int ToCurrencyCode { get; set; }
        public decimal Amount { get; set; }
    }
}
