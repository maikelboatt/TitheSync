using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using TitheSync.ApplicationState.Stores;
using TitheSync.Business.Services.Exports;
using TitheSync.Business.Services.Reports;
using TitheSync.Domain.Enums;
using TitheSync.Infrastructure.Models;

namespace TitheSync.Core.ViewModels
{
    /// <summary>
    ///     ViewModel for comparing Bible class payment reports across different periods and exporting results.
    /// </summary>
    public class BibleClassReportCompareViewModel:MvxViewModel
    {
        /// <summary>
        ///     Service for exporting reports to Excel.
        /// </summary>
        private readonly IExcelExportService _excelExportService;

        /// <summary>
        ///     Store for handling modal navigation.
        /// </summary>
        private readonly IModalNavigationStore _modalNavigationStore;

        /// <summary>
        ///     Service for comparing Bible class reports.
        /// </summary>
        private readonly IReportCompareService _reportCompareService;

        private string _selectedComparisonType;
        private string _selectedPeriod1;
        private string _selectedPeriod2;
        private string _selectedYear1 = "2024";
        private string _selectedYear2 = "2024";

        /// <summary>
        ///     Initializes a new instance of the <see cref="BibleClassReportCompareViewModel" /> class.
        /// </summary>
        /// <param name="reportCompareService" >Service for comparing reports.</param>
        /// <param name="modalNavigationStore" >Store for modal navigation.</param>
        /// <param name="excelExportService" >Service for Excel export.</param>
        public BibleClassReportCompareViewModel( IReportCompareService reportCompareService, IModalNavigationStore modalNavigationStore, IExcelExportService excelExportService )
        {
            _reportCompareService = reportCompareService;
            _modalNavigationStore = modalNavigationStore;
            _excelExportService = excelExportService;
            CompareCommand = new MvxAsyncCommand(CompareReportsAsync);
            CloseCommand = new MvxCommand(ExecuteClose);
            ExportToExcelCommand = new MvxCommand(ExecuteExportToExcel);
            SelectedComparisonType = ComparisonTypes[0];

            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < 10; i++)
            {
                YearOptions.Add((currentYear - i).ToString());
            }
        }

        /// <summary>
        ///     Gets the available comparison types (Quarter, Half-Year, Year).
        /// </summary>
        public ObservableCollection<string> ComparisonTypes { get; } = ["Quarter", "Half-Year", "Year"];

        /// <summary>
        ///     Gets the available year options for selection.
        /// </summary>
        public ObservableCollection<string> YearOptions { get; } = [];

        /// <summary>
        ///     Gets the available period options for the first comparison.
        /// </summary>
        public ObservableCollection<string> PeriodOptions1 { get; } = [];

        /// <summary>
        ///     Gets the available period options for the second comparison.
        /// </summary>
        public ObservableCollection<string> PeriodOptions2 { get; } = [];

        /// <summary>
        ///     Gets the comparison results for the first period.
        /// </summary>
        public ObservableCollection<BibleClassPaymentReport> ComparisonResults1 { get; } = [];

        /// <summary>
        ///     Gets the comparison results for the second period.
        /// </summary>
        public ObservableCollection<BibleClassPaymentReport> ComparisonResults2 { get; } = [];

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
        ///     Gets or sets the selected year for the first period.
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
        ///     Gets or sets the selected year for the second period.
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
        ///     Gets or sets the selected period for the first comparison.
        /// </summary>
        public string SelectedPeriod1
        {
            get => _selectedPeriod1;
            set => SetProperty(ref _selectedPeriod1, value);
        }

        /// <summary>
        ///     Gets or sets the selected period for the second comparison.
        /// </summary>
        public string SelectedPeriod2
        {
            get => _selectedPeriod2;
            set => SetProperty(ref _selectedPeriod2, value);
        }

        /// <summary>
        ///     Command to compare reports for the selected periods.
        /// </summary>
        public IMvxAsyncCommand CompareCommand { get; }

        /// <summary>
        ///     Command to close the modal dialog.
        /// </summary>
        public IMvxCommand CloseCommand { get; }

        /// <summary>
        ///     Command to export the first comparison results to an Excel file.
        /// </summary>
        public IMvxCommand ExportToExcelCommand { get; }

        /// <summary>
        ///     Closes the modal dialog.
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
        ///     Retrieves the Bible class payment reports for a given period and year, and populates the target collection.
        /// </summary>
        /// <param name="selectedComparisonType" >The type of comparison (Quarter, Half-Year, Year).</param>
        /// <param name="selectedPeriod" >The selected period (e.g., Q1 2024).</param>
        /// <param name="selectedYear" >The selected year.</param>
        /// <param name="targetCollection" >The collection to populate with results.</param>
        private void GetReportsForPeriod(
            string selectedComparisonType,
            string selectedPeriod,
            string selectedYear,
            ObservableCollection<BibleClassPaymentReport> targetCollection )
        {
            targetCollection.Clear();
            if (string.IsNullOrEmpty(selectedPeriod) || string.IsNullOrEmpty(selectedYear))
                return;

            int year = int.Parse(selectedYear);

            IEnumerable<(BibleClassEnum ClassName, decimal TotalAmount)> results = Enumerable.Empty<(BibleClassEnum, decimal)>();

            switch (selectedComparisonType)
            {
                case "Quarter":
                {
                    string[] parts = selectedPeriod.Split(' ');
                    int quarter = int.Parse(parts[0][1..]);
                    results = _reportCompareService.GetPaymentsByBibleClassForQuarter(quarter, year);
                    break;
                }
                case "Half-Year":
                {
                    string[] parts = selectedPeriod.Split(' ');
                    int half = int.Parse(parts[0].Substring(1));
                    results = _reportCompareService.GetPaymentsByBibleClassForHalfYear(half, year);
                    break;
                }
                case "Year":
                    results = _reportCompareService.GetPaymentsByBibleClassForYear(year);
                    break;
            }

            foreach ((BibleClassEnum ClassName, decimal TotalAmount) r in results)
                targetCollection.Add(new BibleClassPaymentReport { ClassName = r.ClassName, TotalAmount = r.TotalAmount });
        }

        /// <summary>
        ///     Compares the reports for the selected periods and populates the result collections.
        /// </summary>
        private async Task CompareReportsAsync()
        {
            GetReportsForPeriod(SelectedComparisonType, SelectedPeriod1, SelectedYear1, ComparisonResults1);
            GetReportsForPeriod(SelectedComparisonType, SelectedPeriod2, SelectedYear2, ComparisonResults2);
            await Task.CompletedTask;
        }

        /// <summary>
        ///     Exports the first comparison results to an Excel file.
        /// </summary>
        private void ExecuteExportToExcel()
        {
            _excelExportService.ExportBibleClassReports(ComparisonResults1, "BibleClassReportComparison1.xlsx");
        }
    }
}
