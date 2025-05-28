using MvvmCross.ViewModels;
using TitheSync.Core.Factory;
using TitheSync.Core.ViewModels.Panes;

namespace TitheSync.Core.ViewModels
{
    public class NotificationViewModel( IViewModelFactory viewModelFactory ):MvxViewModel
    {
        public NotificationPaneViewModel? NotificationPaneViewModel => CreateViewModel();

        public NotificationPaneViewModel? CreateViewModel()
        {
            NotificationPaneViewModel? viewModel = viewModelFactory.CreateViewModel<NotificationPaneViewModel>();
            viewModel?.Initialize();
            return viewModel;
        }
    }
}
