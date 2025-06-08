using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using MvvmCross.ViewModels;
using TitheSync.ApplicationState.Stores.Members;
using TitheSync.ApplicationState.Stores.Payments;
using TitheSync.Business.Services.Members;
using TitheSync.Business.Services.Payments;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;

namespace TitheSync.Core.ViewModels.Panes
{
    /// <summary>
    ///     ViewModel for displaying a chart pane with tithe payment summaries by Bible class.
    /// </summary>
    public class ChartPaneViewModel:MvxViewModel
    {
        private readonly IMemberService _memberService;
        private readonly IMemberStore _memberStore;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentStore _paymentStore;
        private bool _isLoading;
        private IEnumerable<string> _messages;

        private ISeries[] _series;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChartPaneViewModel" /> class.
        /// </summary>
        /// <param name="memberService" >Service for member operations.</param>
        /// <param name="paymentService" >Service for payment operations.</param>
        /// <param name="memberStore" >Store for member data.</param>
        /// <param name="paymentStore" >Store for payment data.</param>
        public ChartPaneViewModel(
            IMemberService memberService,
            IPaymentService paymentService,
            IMemberStore memberStore,
            IPaymentStore paymentStore )
        {
            _memberService = memberService;
            _paymentService = paymentService;
            _memberStore = memberStore;
            _paymentStore = paymentStore;
        }

        /// <summary>
        ///     Gets the summary messages for the chart.
        /// </summary>
        public IEnumerable<string> Messages
        {
            get => _messages;
            private set => SetProperty(ref _messages, value);
        }

        /// <summary>
        ///     Gets or sets the chart series data.
        /// </summary>
        public ISeries[] Series
        {
            get => _series;
            set => SetProperty(ref _series, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether data is loading.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        ///     Initializes the ViewModel by loading members and payments, updating the chart, and setting summary messages.
        /// </summary>
        public override async Task Initialize()
        {
            // Load members and payments from the database
            await _memberService.GetMembersAsync();
            await _paymentService.GetPaymentsWithNamesAsync();
            IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> totals = GetTotalPaymentsByBibleClass();
            IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> totalsByClass = totals.ToList();
            UpdateSeries(totalsByClass);
            Messages = GetTotalsSummaryLines(totalsByClass);
            await base.Initialize();
        }

        /// <summary>
        ///     Updates the chart series based on the totals by Bible class.
        /// </summary>
        /// <param name="totalsByClass" >The totals by Bible class.</param>
        private void UpdateSeries( IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> totalsByClass )
        {
            Series = totalsByClass
                     .Select(x => new PieSeries<decimal>
                     {
                         Values = [x.TotalAmount],
                         Name = x.BibleClass.ToString()
                     })
                     .Cast<ISeries>()
                     .ToArray();
        }

        /// <summary>
        ///     Generates summary lines for each Bible class and its total amount.
        /// </summary>
        /// <param name="totals" >The totals by Bible class.</param>
        /// <returns>Enumerable of summary strings.</returns>
        private IEnumerable<string> GetTotalsSummaryLines( IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> totals )
        {
            return totals.Select(x => $"{x.BibleClass}: ₵{x.TotalAmount:N2}");
        }

        /// <summary>
        ///     Calculates the total payments grouped by Bible class.
        /// </summary>
        /// <returns>Enumerable of tuples containing Bible class and total amount.</returns>
        private IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> GetTotalPaymentsByBibleClass()
        {
            IsLoading = true;
            IEnumerable<Member> members = _memberStore.Members;
            IReadOnlyList<PaymentWithName> payments = _paymentStore.PaymentWithNames;

            IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> totalsByClass = members
                                                                                          .GroupJoin(
                                                                                              payments,
                                                                                              member => member.MemberId,
                                                                                              payment => payment.PaymentMemberId,
                                                                                              ( member, memberPayments ) => new
                                                                                              {
                                                                                                  member.BibleClass,
                                                                                                  TotalAmount = memberPayments.Sum(p => p.Amount)
                                                                                              }
                                                                                          )
                                                                                          .GroupBy(x => x.BibleClass)
                                                                                          .Select(g => (BibleClass: g.Key, TotalAmount: g.Sum(x => x.TotalAmount)));

            IsLoading = false;
            return totalsByClass;
        }
    }
}
