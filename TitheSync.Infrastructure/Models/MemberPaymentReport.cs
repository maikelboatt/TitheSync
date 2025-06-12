namespace TitheSync.Infrastructure.Models
{
    public class MemberPaymentReport
    {
        public string FullName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
