using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using TitheSync.ApplicationState.Stores;
using TitheSync.Business.Services.Reports;
using TitheSync.Domain.Enums;
using TitheSync.Infrastructure.Models;

namespace TitheSync.Core.ViewModels
{
    public class BibleClassReportCompareViewModel:MvxViewModel
    {
        private readonly IModalNavigationStore _modalNavigationStore;
        private readonly IReportCompareService _reportCompareService;

        private string _selectedComparisonType;
        private string _selectedPeriod1;
        private string _selectedPeriod2;
        private string _selectedYear1 = "2024";
        private string _selectedYear2 = "2024";

        public BibleClassReportCompareViewModel( IReportCompareService reportCompareService, IModalNavigationStore modalNavigationStore )
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

        public ObservableCollection<string> ComparisonTypes { get; } = ["Quarter", "Half-Year", "Year"];
        public ObservableCollection<string> YearOptions { get; } = [];
        public ObservableCollection<string> PeriodOptions1 { get; } = [];
        public ObservableCollection<string> PeriodOptions2 { get; } = [];
        public ObservableCollection<BibleClassPaymentReport> ComparisonResults1 { get; } = [];
        public ObservableCollection<BibleClassPaymentReport> ComparisonResults2 { get; } = [];

        public string SelectedComparisonType
        {
            get => _selectedComparisonType;
            set
            {
                SetProperty(ref _selectedComparisonType, value);
                UpdatePeriodOptions();
            }
        }

        public string SelectedYear1
        {
            get => _selectedYear1;
            set
            {
                SetProperty(ref _selectedYear1, value);
                UpdatePeriodOptions();
            }
        }

        public string SelectedYear2
        {
            get => _selectedYear2;
            set
            {
                SetProperty(ref _selectedYear2, value);
                UpdatePeriodOptions();
            }
        }

        public string SelectedPeriod1
        {
            get => _selectedPeriod1;
            set => SetProperty(ref _selectedPeriod1, value);
        }

        public string SelectedPeriod2
        {
            get => _selectedPeriod2;
            set => SetProperty(ref _selectedPeriod2, value);
        }

        public IMvxAsyncCommand CompareCommand { get; }
        public IMvxCommand CloseCommand { get; }

        private void ExecuteClose()
        {
            _modalNavigationStore.Close();
        }

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

        private void GetReportsForPeriod( string selectedComparisonType, string selectedPeriod, string selectedYear, ObservableCollection<BibleClassPaymentReport> targetCollection )
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

        private async Task CompareReportsAsync()
        {
            GetReportsForPeriod(SelectedComparisonType, SelectedPeriod1, SelectedYear1, ComparisonResults1);
            GetReportsForPeriod(SelectedComparisonType, SelectedPeriod2, SelectedYear2, ComparisonResults2);
            await Task.CompletedTask;
        }
    }
}
