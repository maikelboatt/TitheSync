using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using MvvmCross.ViewModels;
using TitheSync.Business.Services.Reports;
using TitheSync.Domain.Enums;

namespace TitheSync.Core.ViewModels.Panes
{
    /// <summary>
    ///     ViewModel for displaying a chart pane with tithe payment summaries by Bible class.
    /// </summary>
    public class ChartPaneViewModel:MvxViewModel
    {
        /// <summary>
        ///     Service for generating reports.
        /// </summary>
        private readonly IReportingService _reportingService;

        /// <summary>
        ///     Stores totals for Bible classes over different periods (Quarterly, SemiAnnual, Yearly).
        /// </summary>
        private (IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Quarterly,
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> SemiAnnual,
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Yearly) _classOverPeriodTotals;

        /// <summary>
        ///     Indicates whether data is currently loading.
        /// </summary>
        private bool _isLoading;

        /// <summary>
        ///     Stores summary messages for the chart.
        /// </summary>
        private IEnumerable<string> _messages;

        /// <summary>
        ///     Stores quarterly totals by Bible class.
        /// </summary>
        private IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> _quarterlyTotals;

        /// <summary>
        ///     Stores semi-annual totals by Bible class.
        /// </summary>
        private IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> _semiAnnualTotals;

        /// <summary>
        ///     Stores the chart series data.
        /// </summary>
        private ISeries[] _series;

        /// <summary>
        ///     Stores the top ten payers in the current quarter.
        /// </summary>
        private IEnumerable<(string Fullname, decimal TotalAmount)> _topTenPayersInCurrentQuarter;

        /// <summary>
        ///     Stores yearly totals by Bible class.
        /// </summary>
        private IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> _yearlyTotals;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChartPaneViewModel" /> class.
        /// </summary>
        /// <param name="reportingService" >Service for generating reports</param>
        public ChartPaneViewModel( IReportingService reportingService )
        {
            _reportingService = reportingService;
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
        ///     Gets or sets the top ten payers in the current quarter.
        /// </summary>
        public IEnumerable<(string Fullname, decimal TotalAmount)> TopTenPayersInCurrentQuarter
        {
            get => _topTenPayersInCurrentQuarter;
            private set => SetProperty(ref _topTenPayersInCurrentQuarter, value);
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
            await _reportingService.GetDataFromDb();
            PopulateCharts();
            UpdateSeries(_quarterlyTotals);
            await base.Initialize();
        }

        /// <summary>
        ///     Populates the chart data and top ten payers from the reporting service.
        /// </summary>
        private void PopulateCharts()
        {
            IsLoading = true;
            // Load totals by Bible class for different periods
            (IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Quarterly,
                IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> SemiAnnual,
                IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Yearly) totals = _reportingService.GetTotalPaymentsByBibleClassPeriods();

            _classOverPeriodTotals = totals;
            AssignPeriodTotals();

            // Assign the totals to the respective fields
            Messages = _reportingService.GetTotalsSummaryLines(_quarterlyTotals);

            // Load the top ten payers in the current quarter
            List<(string FullName, decimal TotalAmount)> topTenPayers = _reportingService.GetTopTenPayersInCurrentQuarter();
            TopTenPayersInCurrentQuarter = topTenPayers;

            IsLoading = false;
        }

        /// <summary>
        ///     Assigns the period totals from the class over period totals.
        /// </summary>
        private void AssignPeriodTotals()
        {
            _quarterlyTotals = _classOverPeriodTotals.Quarterly;
            _semiAnnualTotals = _classOverPeriodTotals.SemiAnnual;
            _yearlyTotals = _classOverPeriodTotals.Yearly;
        }

        /// <summary>
        ///     Updates the chart series based on the totals by Bible class.
        /// </summary>
        /// <param name="totalsByClass" >The totals by Bible class.</param>
        private void UpdateSeries( IEnumerable<(BibleClassEnum BibleClass, string period, decimal TotalAmount)> totalsByClass )
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
    }
}
