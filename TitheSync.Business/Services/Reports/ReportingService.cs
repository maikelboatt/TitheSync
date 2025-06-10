using TitheSync.ApplicationState.Stores.Members;
using TitheSync.ApplicationState.Stores.Payments;
using TitheSync.Business.Services.Members;
using TitheSync.Business.Services.Payments;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Reports
{
    public class ReportingService:IReportingService
    {
        /// <summary>
        ///     Service for generating various reports related to members and payments.
        /// </summary>
        private readonly IMemberService _memberService;

        private readonly IMemberStore _memberStore;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentStore _paymentStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportingService" /> class.
        /// </summary>
        /// <param name="memberService" >Service for member operations.</param>
        /// <param name="paymentService" >Service for payment operations.</param>
        /// <param name="memberStore" >Store for member data.</param>
        /// <param name="paymentStore" >Store for payment data.</param>
        public ReportingService( IMemberService memberService, IPaymentService paymentService, IMemberStore memberStore, IPaymentStore paymentStore )
        {
            _memberService = memberService;
            _paymentService = paymentService;
            _memberStore = memberStore;
            _paymentStore = paymentStore;
        }

        /// <summary>
        ///     Generates summary lines for each Bible class and its total amount.
        /// </summary>
        /// <param name="totals" >The totals by Bible class.</param>
        /// <returns>Enumerable of summary strings.</returns>
        public IEnumerable<string> GetTotalsSummaryLines( IEnumerable<(BibleClassEnum BibleClass, string period, decimal TotalAmount)> totals )
        {
            return totals.Select(x => $"{x.BibleClass}: ₵{x.TotalAmount:N2} over {x.period}");
        }

        /// <summary>
        ///     Loads members and payments from the database asynchronously.
        /// </summary>
        public async Task GetDataFromDb()
        {
            // Load members and payments from the database
            await _memberService.GetMembersAsync();
            await _paymentService.GetPaymentsWithNamesAsync();
        }

        // private (IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Quarterly,
        //     IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> SemiAnnual,
        //     IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Yearly)
        //     GetTotalPaymentsByBibleClassPeriods()
        // {
        //     IsLoading = true;
        //     var members = _memberStore.Members;
        //     var payments = _paymentStore.PaymentWithNames;
        //     //
        //     // Helper to get period label
        //     string GetQuarter(int month) => $"Q{((month - 1) / 3) + 1}";
        //     string GetSemiAnnual(int month) => (month <= 6) ? "H1" : "H2";
        //     string GetYear(int year) => year.ToString();
        //     //
        //     // Join members and payments
        //     var joined = from member in members
        //                  join payment in payments on member.MemberId equals payment.PaymentMemberId
        //                  select new
        //                  {
        //                      member.BibleClass,
        //                      payment.Amount,
        //                      payment.PaymentDate
        //                  };
        //     //
        //     // Quarterly
        //     var quarterly = joined
        //                     .GroupBy(x => new { x.BibleClass, Period = $"{GetQuarter(x.PaymentDate.Month)} {x.PaymentDate.Year}" })
        //                     .Select(g => (g.Key.BibleClass, g.Key.Period, TotalAmount: g.Sum(x => x.Amount)));
        //     //
        //     // Semi-Annual
        //     var semiAnnual = joined
        //                      .GroupBy(x => new { x.BibleClass, Period = $"{GetSemiAnnual(x.PaymentDate.Month)} {x.PaymentDate.Year}" })
        //                      .Select(g => (g.Key.BibleClass, g.Key.Period, TotalAmount: g.Sum(x => x.Amount)));
        //     //
        //     // Yearly
        //     var yearly = joined
        //                  .GroupBy(x => new { x.BibleClass, Period = GetYear(x.PaymentDate.Year) })
        //                  .Select(g => (g.Key.BibleClass, g.Key.Period, TotalAmount: g.Sum(x => x.Amount)));
        //     //
        //     IsLoading = false;
        //     return (quarterly, semiAnnual, yearly);
        // }

        /// <summary>
        ///     Gets the total payments by Bible class for the current quarter, current half-year, and all years.
        /// </summary>
        /// <returns>
        ///     A tuple containing:
        ///     - Quarterly totals for the current quarter,
        ///     - Semi-annual totals for the current half,
        ///     - Yearly totals for all years.
        /// </returns>
        public (IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Quarterly,
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> SemiAnnual,
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Yearly)
            GetTotalPaymentsByBibleClassPeriods()
        {
            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;

            var joined = from member in members
                         join payment in payments on member.MemberId equals payment.PaymentMemberId
                         select new
                         {
                             member.BibleClass,
                             payment.Amount,
                             payment.DatePaid
                         };

            // Helpers
            string currentQuarterLabel = $"{CurrentQuarter}nd Qtr {DateTime.Now.Year}";
            string GetQuarter( int month, int year ) => $"{(month - 1) / 3 + 1}nd Qtr {year}";

            string currentSemiAnnualLabel = (DateTime.Now.Month <= 6 ? "1st Half" : "2nd Half") + $" {DateTime.Now.Year}";
            string GetSemiAnnual( int month, int year ) => (month <= 6 ? "1st Half" : "2nd Half") + $" {year}";

            string GetYear( int year ) => year.ToString();

            var enumerable = joined.ToList();

            // Quarterly: Only current quarter
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> quarterly = enumerable
                                                                                                     .GroupBy(x => new
                                                                                                     {
                                                                                                         x.BibleClass, Period = GetQuarter(x.DatePaid.Month, x.DatePaid.Year)
                                                                                                     })
                                                                                                     .Where(g => g.Key.Period == currentQuarterLabel)
                                                                                                     .Select(g => (g.Key.BibleClass, g.Key.Period, TotalAmount: g.Sum(x => x.Amount)));

            // Semi-Annual: Only current half
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> semiAnnual = enumerable
                                                                                                      .GroupBy(x => new
                                                                                                      {
                                                                                                          x.BibleClass, Period = GetSemiAnnual(x.DatePaid.Month, x.DatePaid.Year)
                                                                                                      })
                                                                                                      .Where(g => g.Key.Period == currentSemiAnnualLabel)
                                                                                                      .Select(g => (g.Key.BibleClass, g.Key.Period, TotalAmount: g.Sum(x => x.Amount)));

            // Yearly: All years
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> yearly = enumerable
                                                                                                  .GroupBy(x => new { x.BibleClass, Period = GetYear(x.DatePaid.Year) })
                                                                                                  .Select(g => (g.Key.BibleClass, g.Key.Period, TotalAmount: g.Sum(x => x.Amount)));

            return (quarterly, semiAnnual, yearly);
        }

        /// <summary>
        ///     Retrieves the top ten payers in the current quarter based on total payment amounts.
        /// </summary>
        /// <remarks>
        ///     This method filters payments to only those made in the current quarter and year,
        ///     groups them by member, sums the total amount per member, and returns the top ten
        ///     payers along with their full names and total amounts.
        /// </remarks>
        /// <returns>
        ///     A list of tuples containing the full name and total amount paid by the top ten payers
        ///     in the current quarter.
        /// </returns>
        public List<(string FullName, decimal TotalAmount)> GetTopTenPayersInCurrentQuarter()
        {
            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;
        
            // Filter payments in current quarter
            IEnumerable<PaymentWithName> paymentsInQuarter = payments
                .Where(p =>
                           p.DatePaid.Year == CurrentYear &&
                           (p.DatePaid.Month - 1) / 3 + 1 == CurrentQuarter);
        
            // Group by member and sum
            var totalsByMember = paymentsInQuarter
                                 .GroupBy(p => p.PaymentMemberId)
                                 .Select(g => new
                                 {
                                     MemberId = g.Key,
                                     TotalAmount = g.Sum(x => x.Amount)
                                 })
                                 .OrderByDescending(x => x.TotalAmount)
                                 .Take(10)
                                 .ToList();
        
            // Join with members and select names
            IEnumerable<(string FirstName, string LastName, decimal TotalAmount)> topPayers = totalsByMember
                .Join(members, t => t.MemberId, m => m.MemberId, ( t, m ) => (m.FirstName, m.LastName, t.TotalAmount));
        
            List<(string FullName, decimal TotalAmount)> result = topPayers
                                                                  .Select(x => (FullName: $"{x.FirstName} {x.LastName}", x.TotalAmount))
                                                                  .ToList();
        
            return result;
        }

        #region Properties

        /// <summary>
        ///     Gets the current quarter (1-4).
        /// </summary>
        private int CurrentQuarter { get; } = (DateTime.Now.Month - 1) / 3 + 1;

        /// <summary>
        ///     Gets the current year.
        /// </summary>
        private int CurrentYear { get; } = DateTime.Now.Year;

        #endregion
    }
}
