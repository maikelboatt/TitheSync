using MvvmCross.ViewModels;
using TitheSync.Core.ViewModels.Panes;

namespace TitheSync.Core.ViewModels
{
    public class DashboardViewModel:MvxViewModel
    {
        public ChartPaneViewModel? ChartPaneViewModel { get; set; }
    }
}
