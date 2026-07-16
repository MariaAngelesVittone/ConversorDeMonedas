using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Currency
    {
        [Key]
        public int Code { get; set; }
        public string Leyend { get; set; }

        public string Symbol { get; set; }

        public decimal ConversionRate { get; set; }

        public CurrencyState State { get; set; }
    }
}
