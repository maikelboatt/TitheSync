using MvvmCross.ViewModels;
using TitheSync.Core.Factory;
using TitheSync.Core.ViewModels.Panes;

namespace TitheSync.Core.ViewModels
{
    public class DashboardViewModel( IViewModelFactory viewModelFactory ):MvxViewModel
    {
        public ChartPaneViewModel? ChartPaneViewModel => CreateChartPaneViewModel();

        private ChartPaneViewModel? CreateChartPaneViewModel()
        {
            ChartPaneViewModel? viewModel = viewModelFactory.CreateViewModel<ChartPaneViewModel>();
            viewModel?.Initialize();
            return viewModel;
        }
    }
}
