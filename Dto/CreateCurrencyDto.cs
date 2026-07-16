namespace Dto
{
    public class CreateCurrencyDto
    {
        public int Codigo { get; set; }
        public string Leyenda { get; set; } = string.Empty;
        public string? Simbolo { get; set; }
        public decimal idc { get; set; }
    }
}
