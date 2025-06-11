using TitheSync.ApplicationState.Stores.Members;
using TitheSync.ApplicationState.Stores.Payments;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Reports
{
    public class ReportCompareService:IReportCompareService
    {
        private readonly IMemberStore _memberStore;
        private readonly IPaymentStore _paymentStore;

        public ReportCompareService( IMemberStore memberStore, IPaymentStore paymentStore )
        {
            _memberStore = memberStore;
            _paymentStore = paymentStore;
        }

        #region Compare Payments by Member

        public IEnumerable<(string FullName, decimal TotalAmount)> GetPaymentsByMemberForQuarter( int quarter, int? year = null )
        {
            int targetYear = year ?? DateTime.Now.Year;
            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;

            IEnumerable<PaymentWithName> paymentsInQuarter = payments
                .Where(p =>
                           p.DatePaid.Year == targetYear &&
                           (p.DatePaid.Month - 1) / 3 + 1 == quarter);

            var joined = from member in members
                         join payment in paymentsInQuarter on member.MemberId equals payment.PaymentMemberId
                         select new
                         {
                             FullName = member.FirstName + " " + member.LastName,
                             payment.Amount
                         };

            return joined
                   .GroupBy(x => x.FullName)
                   .Select(g => (g.Key, TotalAmount: g.Sum(x => x.Amount)))
                   .OrderByDescending(x => x.TotalAmount);
        }

        public IEnumerable<(string FullName, decimal TotalAmount)> GetPaymentsByMemberForHalfYear( int half, int? year = null )
        {
            int targetYear = year ?? DateTime.Now.Year;
            int startMonth = half == 1 ? 1 : 7;
            int endMonth = half == 1 ? 6 : 12;
            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;

            IEnumerable<PaymentWithName> paymentsInHalf = payments
                .Where(p =>
                           p.DatePaid.Year == targetYear &&
                           p.DatePaid.Month >= startMonth &&
                           p.DatePaid.Month <= endMonth);

            var joined = from member in members
                         join payment in paymentsInHalf on member.MemberId equals payment.PaymentMemberId
                         select new
                         {
                             FullName = member.FirstName + " " + member.LastName,
                             payment.Amount
                         };

            return joined
                   .GroupBy(x => x.FullName)
                   .Select(g => (g.Key, TotalAmount: g.Sum(x => x.Amount)))
                   .OrderByDescending(x => x.TotalAmount);
        }

        public IEnumerable<(string FullName, decimal TotalAmount)> GetPaymentsByMemberForYear( int year )
        {
            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;

            IEnumerable<PaymentWithName> paymentsInYear = payments
                .Where(p => p.DatePaid.Year == year);

            var joined = from member in members
                         join payment in paymentsInYear on member.MemberId equals payment.PaymentMemberId
                         select new
                         {
                             FullName = member.FirstName + " " + member.LastName,
                             payment.Amount
                         };

            return joined
                   .GroupBy(x => x.FullName)
                   .Select(g => (g.Key, TotalAmount: g.Sum(x => x.Amount)))
                   .OrderByDescending(x => x.TotalAmount);
        }

        #endregion


        #region Compare Payments by Bible Class

        public IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> GetPaymentsByBibleClassForQuarter( int quarter, int? year = null )
        {
            int targetYear = year ?? DateTime.Now.Year;
            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;

            IEnumerable<PaymentWithName> paymentsInQuarter = payments
                .Where(p =>
                           p.DatePaid.Year == targetYear &&
                           (p.DatePaid.Month - 1) / 3 + 1 == quarter);

            var joined = from member in members
                         join payment in paymentsInQuarter on member.MemberId equals payment.PaymentMemberId
                         select new
                         {
                             member.BibleClass,
                             payment.Amount
                         };

            IOrderedEnumerable<(BibleClassEnum Key, decimal TotalAmount)> grouped = joined
                                                                                    .GroupBy(x => x.BibleClass)
                                                                                    .Select(g => (g.Key, TotalAmount: g.Sum(x => x.Amount)))
                                                                                    .OrderBy(x => x.Key);

            return grouped;
        }

        public IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> GetPaymentsByBibleClassForHalfYear( int half, int? year = null )
        {
            int targetYear = year ?? DateTime.Now.Year;
            int startMonth = half == 1 ? 1 : 7;
            int endMonth = half == 1 ? 6 : 12;

            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;

            IEnumerable<PaymentWithName> paymentsInHalf = payments
                .Where(p =>
                           p.DatePaid.Year == targetYear &&
                           p.DatePaid.Month >= startMonth &&
                           p.DatePaid.Month <= endMonth);

            var joined = from member in members
                         join payment in paymentsInHalf on member.MemberId equals payment.PaymentMemberId
                         select new
                         {
                             member.BibleClass,
                             payment.Amount
                         };

            IOrderedEnumerable<(BibleClassEnum Key, decimal TotalAmount)> grouped = joined
                                                                                    .GroupBy(x => x.BibleClass)
                                                                                    .Select(g => (g.Key, TotalAmount: g.Sum(x => x.Amount)))
                                                                                    .OrderBy(x => x.Key);

            return grouped;
        }

        public IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> GetPaymentsByBibleClassForYear( int year )
        {
            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;

            IEnumerable<PaymentWithName> paymentsInYear = payments
                .Where(p => p.DatePaid.Year == year);

            var joined = from member in members
                         join payment in paymentsInYear on member.MemberId equals payment.PaymentMemberId
                         select new
                         {
                             member.BibleClass,
                             payment.Amount
                         };

            IOrderedEnumerable<(BibleClassEnum Key, decimal TotalAmount)> grouped = joined
                                                                                    .GroupBy(x => x.BibleClass)
                                                                                    .Select(g => (g.Key, TotalAmount: g.Sum(x => x.Amount)))
                                                                                    .OrderBy(x => x.Key);

            return grouped;
        }

        #endregion
    }
}
