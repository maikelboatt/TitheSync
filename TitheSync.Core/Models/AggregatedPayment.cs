namespace TitheSync.Core.Models
{
    public class AggregatedPayment
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public decimal TotalAmount { get; set; }

        public string CurrentMonth { get; set; } = string.Empty;
    }
}
