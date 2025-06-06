using LiveCharts;
using LiveCharts.Defaults;
using MvvmCross.ViewModels;
using TitheSync.ApplicationState.Stores.Members;
using TitheSync.Business.Services.Members;

namespace TitheSync.Core.ViewModels.Panes
{
    public class ChartPaneViewModel:MvxViewModel
    {
        private readonly double _abrahamDadzie = 14d;
        private readonly double _atoPrempeh = 15d;
        private readonly double _auntyAggie = 12d;
        private readonly double _emeliaAkrofi = 20d;
        private readonly double _jacobBiney = 10d;
        private readonly IMemberService _memberService;
        private readonly IMemberStore _memberStore;
        private readonly double _michaelKumi = 18d;
        private readonly double _mrLartey = 22d;
        private readonly double _profDanso = 25d;

        private IChartValues _abrahamDadzieValues;
        private IChartValues _atoPrempehValues;
        private IChartValues _auntyAggieValues;
        private IChartValues _emeliaAkrofiValues;

        private bool _isLoading;

        private IChartValues _jacobBineyValues;
        private IChartValues _michaelKumiValues;
        private IChartValues _mrLarteyValues;
        private IChartValues _profDansoValues;

        public ChartPaneViewModel( IMemberService memberService, IMemberStore memberStore )
        {
            _memberService = memberService;
            _memberStore = memberStore;
            _abrahamDadzieValues = new ChartValues<ObservableValue> { new(_abrahamDadzie) };
            _atoPrempehValues = new ChartValues<ObservableValue> { new(_atoPrempeh) };
            _auntyAggieValues = new ChartValues<ObservableValue> { new(_auntyAggie) };
            _emeliaAkrofiValues = new ChartValues<ObservableValue> { new(_emeliaAkrofi) };
            _jacobBineyValues = new ChartValues<ObservableValue> { new(_jacobBiney) };
            _michaelKumiValues = new ChartValues<ObservableValue> { new(_michaelKumi) };
            _mrLarteyValues = new ChartValues<ObservableValue> { new(_mrLartey) };
            _profDansoValues = new ChartValues<ObservableValue> { new(_profDanso) };
        }

        public Func<ChartPoint, string> PointLabel { get; set; }
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }


        public override void Prepare()
        {
            SetupPieChart();
            base.Prepare();
        }

        public override async Task Initialize()
        {
            IsLoading = true;
            try
            {
                await Task.Delay(3000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            IsLoading = false;

            await base.Initialize();
        }

        private void SetupPieChart()
        {

            PointLabel = point => $"{point.Y} ({point.Participation:P})";
        }

        #region Chart Values

        // public IChartValues ProfDansoValues
        // {
        //     get
        //     {
        //         ObservableValue count = new(_profDanso);
        //         return new ChartValues<ObservableValue>([count]);
        //     }
        //     set => SetProperty(ref _profDansoValues, value);
        // }
        //
        // public IChartValues MrLarteyValues
        // {
        //     get
        //     {
        //         ObservableValue count = new(_mrLartey);
        //         return new ChartValues<ObservableValue>([count]);
        //     }
        //     set => SetProperty(ref _mrLarteyValues, value);
        // }
        //
        // public IChartValues MichaelKumiValues
        // {
        //     get
        //     {
        //         ObservableValue count = new(_michaelKumi);
        //         return new ChartValues<ObservableValue>([count]);
        //     }
        //     set => SetProperty(ref _michaelKumiValues, value);
        // }
        //
        // public IChartValues JacobBineyValues
        // {
        //     get
        //     {
        //         ObservableValue count = new(_jacobBiney);
        //         return new ChartValues<ObservableValue>([count]);
        //     }
        //     set => SetProperty(ref _jacobBineyValues, value);
        // }
        //
        // public IChartValues EmeliaAkrofiValues
        // {
        //     get
        //     {
        //         ObservableValue count = new(_emeliaAkrofi);
        //         return new ChartValues<ObservableValue>([count]);
        //     }
        //     set => SetProperty(ref _emeliaAkrofiValues, value);
        // }
        //
        // public IChartValues AuntyAggieValues
        // {
        //     get
        //     {
        //         ObservableValue count = new(_auntyAggie);
        //         return new ChartValues<ObservableValue>([count]);
        //     }
        //     set => SetProperty(ref _auntyAggieValues, value);
        // }
        //
        // public IChartValues AtoPrempehValues
        // {
        //     get
        //     {
        //         ObservableValue count = new(_atoPrempeh);
        //         return new ChartValues<ObservableValue>([count]);
        //     }
        //     set => SetProperty(ref _atoPrempehValues, value);
        // }
        //
        // public IChartValues AbrahamDadzieValues
        // {
        //     get
        //     {
        //         ObservableValue count = new(_abrahamDadzie);
        //         return new ChartValues<ObservableValue>([count]);
        //     }
        //     set => SetProperty(ref _abrahamDadzieValues, value);
        // }
        public IChartValues ProfDansoValues
        {
            get => _profDansoValues;
            set => SetProperty(ref _profDansoValues, value);
        }

        public IChartValues MrLarteyValues
        {
            get => _mrLarteyValues;
            set => SetProperty(ref _mrLarteyValues, value);
        }

        public IChartValues MichaelKumiValues
        {
            get => _michaelKumiValues;
            set => SetProperty(ref _michaelKumiValues, value);
        }

        public IChartValues JacobBineyValues
        {
            get => _jacobBineyValues;
            set => SetProperty(ref _jacobBineyValues, value);
        }

        public IChartValues EmeliaAkrofiValues
        {
            get => _emeliaAkrofiValues;
            set => SetProperty(ref _emeliaAkrofiValues, value);
        }

        public IChartValues AuntyAggieValues
        {
            get => _auntyAggieValues;
            set => SetProperty(ref _auntyAggieValues, value);
        }

        public IChartValues AtoPrempehValues
        {
            get => _atoPrempehValues;
            set => SetProperty(ref _atoPrempehValues, value);
        }

        public IChartValues AbrahamDadzieValues
        {
            get => _abrahamDadzieValues;
            set => SetProperty(ref _abrahamDadzieValues, value);
        }

        #endregion
    }
}
