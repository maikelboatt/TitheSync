using TitheSync.Domain.Enums;

namespace TitheSync.ApplicationState.Stores.Reports
{
    public class ReportStore:IReportStore
    {
        public IEnumerable<(string FullName, decimal TotalAmount)> QuarterPaymentsForMembers { get; set; }
        public IEnumerable<(string FullName, decimal TotalAmount)> SemiAnnualPaymentsForMembers { get; set; }
        public IEnumerable<(string FullName, decimal TotalAmount)> AnnualPaymentsForMembers { get; set; }
        public IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> QuarterPaymentsForBibleClasses { get; set; }
        public IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> SemiAnnualPaymentsForBibleClasses { get; set; }
        public IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> AnnualPaymentsForBibleClasses { get; set; }
    }
}
