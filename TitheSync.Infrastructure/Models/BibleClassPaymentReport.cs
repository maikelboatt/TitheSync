using TitheSync.Domain.Enums;

namespace TitheSync.Infrastructure.Models
{
    public class BibleClassPaymentReport
    {
        public BibleClassEnum ClassName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
