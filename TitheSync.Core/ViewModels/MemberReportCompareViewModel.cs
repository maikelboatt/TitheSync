using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using TitheSync.ApplicationState.Stores;
using TitheSync.Business.Services.Reports;
using TitheSync.Infrastructure.Models;

namespace TitheSync.Core.ViewModels
{
    /// <summary>
    ///     ViewModel for comparing member payment reports across different periods and types (Quarter, Half-Year, Year).
    ///     Handles selection of comparison types, years, and periods, and retrieves comparison results.
    /// </summary>
    public class MemberReportCompareViewModel:MvxViewModel
    {
        private readonly IModalNavigationStore _modalNavigationStore;
        private readonly IReportCompareService _reportCompareService;

        private string _selectedComparisonType;
        private string _selectedPeriod1;
        private string _selectedPeriod2;
        private string _selectedYear1 = "2024";
        private string _selectedYear2 = "2024";

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberReportCompareViewModel" /> class.
        ///     Sets up commands and initializes year options.
        /// </summary>
        /// <param name="reportCompareService" >Service for comparing reports.</param>
        /// <param name="modalNavigationStore" >Navigation store for modal dialogs.</param>
        public MemberReportCompareViewModel( IReportCompareService reportCompareService, IModalNavigationStore modalNavigationStore )
        {
            _reportCompareService = reportCompareService;
            _modalNavigationStore = modalNavigationStore;
            CompareCommand = new MvxAsyncCommand(CompareReportsAsync);
            CloseCommand = new MvxCommand(ExecuteClose);
            SelectedComparisonType = ComparisonTypes[0];

            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < 10; i++)
            {
                YearOptions.Add((currentYear - i).ToString());
            }
        }

        /// <summary>
        ///     Gets the available comparison types.
        /// </summary>
        public ObservableCollection<string> ComparisonTypes { get; } = ["Quarter", "Half-Year", "Year"];

        /// <summary>
        ///     Gets or sets the selected comparison type.
        /// </summary>
        public string SelectedComparisonType
        {
            get => _selectedComparisonType;
            set
            {
                SetProperty(ref _selectedComparisonType, value);
                UpdatePeriodOptions();
            }
        }

        /// <summary>
        ///     Gets the available year options.
        /// </summary>
        public ObservableCollection<string> YearOptions { get; } = [];

        /// <summary>
        ///     Gets or sets the first selected year.
        /// </summary>
        public string SelectedYear1
        {
            get => _selectedYear1;
            set
            {
                SetProperty(ref _selectedYear1, value);
                UpdatePeriodOptions();
            }
        }

        /// <summary>
        ///     Gets or sets the second selected year.
        /// </summary>
        public string SelectedYear2
        {
            get => _selectedYear2;
            set
            {
                SetProperty(ref _selectedYear2, value);
                UpdatePeriodOptions();
            }
        }

        /// <summary>
        ///     Gets the available period options for the first selection.
        /// </summary>
        public ObservableCollection<string> PeriodOptions1 { get; } = [];

        /// <summary>
        ///     Gets the available period options for the second selection.
        /// </summary>
        public ObservableCollection<string> PeriodOptions2 { get; } = [];

        /// <summary>
        ///     Gets or sets the first selected period.
        /// </summary>
        public string SelectedPeriod1
        {
            get => _selectedPeriod1;
            set => SetProperty(ref _selectedPeriod1, value);
        }

        /// <summary>
        ///     Gets or sets the second selected period.
        /// </summary>
        public string SelectedPeriod2
        {
            get => _selectedPeriod2;
            set => SetProperty(ref _selectedPeriod2, value);
        }

        /// <summary>
        ///     Gets the comparison results for the first selection.
        /// </summary>
        public ObservableCollection<MemberPaymentReport> ComparisonResults1 { get; } = [];

        /// <summary>
        ///     Gets the comparison results for the second selection.
        /// </summary>
        public ObservableCollection<MemberPaymentReport> ComparisonResults2 { get; } = [];

        /// <summary>
        ///     Command to compare reports for the selected periods.
        /// </summary>
        public IMvxAsyncCommand CompareCommand { get; }

        /// <summary>
        ///     Command to close the modal dialog.
        /// </summary>
        public IMvxCommand CloseCommand { get; }

        /// <summary>
        ///     Executes the close command, closing the modal dialog.
        /// </summary>
        private void ExecuteClose()
        {
            _modalNavigationStore.Close();
        }

        /// <summary>
        ///     Updates the available period options based on the selected comparison type and years.
        /// </summary>
        private void UpdatePeriodOptions()
        {
            PeriodOptions1.Clear();
            PeriodOptions2.Clear();

            string year1 = SelectedYear1 ?? "2024";
            string year2 = SelectedYear2 ?? "2024";

            switch (SelectedComparisonType)
            {
                case "Quarter":
                    for (int i = 1; i <= 4; i++)
                    {
                        PeriodOptions1.Add($"Q{i} {year1}");
                        PeriodOptions2.Add($"Q{i} {year2}");
                    }
                    break;
                case "Half-Year":
                    PeriodOptions1.Add($"H1 {year1}");
                    PeriodOptions1.Add($"H2 {year1}");
                    PeriodOptions2.Add($"H1 {year2}");
                    PeriodOptions2.Add($"H2 {year2}");
                    break;
                case "Year":
                    PeriodOptions1.Add(year1);
                    PeriodOptions2.Add(year2);
                    break;
            }

            RaisePropertyChanged(() => PeriodOptions1);
            RaisePropertyChanged(() => PeriodOptions2);
        }

        /// <summary>
        ///     Retrieves reports for a given period and year, and populates the target collection.
        /// </summary>
        /// <param name="selectedComparisonType" >The type of comparison (Quarter, Half-Year, Year).</param>
        /// <param name="selectedPeriod" >The selected period string.</param>
        /// <param name="selectedYear" >The selected year string.</param>
        /// <param name="targetCollection" >The collection to populate with results.</param>
        private void GetReportsForPeriod( string selectedComparisonType, string selectedPeriod, string selectedYear, ObservableCollection<MemberPaymentReport> targetCollection )
        {
            targetCollection.Clear();
            if (string.IsNullOrEmpty(selectedPeriod) || string.IsNullOrEmpty(selectedYear))
                return;

            int year = int.Parse(selectedYear);

            IEnumerable<(string FullName, decimal TotalAmount)> results = Enumerable.Empty<(string, decimal)>();

            switch (selectedComparisonType)
            {
                case "Quarter":
                {
                    // selectedPeriod example: "Q1 2023"
                    string[] parts = selectedPeriod.Split(' ');
                    int quarter = int.Parse(parts[0][1..]);
                    results = _reportCompareService.GetPaymentsByMemberForQuarter(quarter, year);
                    break;
                }
                case "Half-Year":
                {
                    // selectedPeriod example: "H1 2023"
                    string[] parts = selectedPeriod.Split(' ');
                    int half = int.Parse(parts[0].Substring(1));
                    results = _reportCompareService.GetPaymentsByMemberForHalfYear(half, year);
                    break;
                }
                case "Year":
                    results = _reportCompareService.GetPaymentsByMemberForYear(year);
                    break;
            }

            foreach ((string FullName, decimal TotalAmount) r in results)
                targetCollection.Add(new MemberPaymentReport { FullName = r.FullName, TotalAmount = r.TotalAmount });
        }

        /// <summary>
        ///     Compares reports for the selected periods and populates the result collections.
        /// </summary>
        private async Task CompareReportsAsync()
        {
            GetReportsForPeriod(SelectedComparisonType, SelectedPeriod1, SelectedYear1, ComparisonResults1);
            GetReportsForPeriod(SelectedComparisonType, SelectedPeriod2, SelectedYear2, ComparisonResults2);
            await Task.CompletedTask;
        }
    }
}
