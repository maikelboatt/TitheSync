using MvvmCross.Commands;
using MvvmCross.ViewModels;
using TitheSync.Core.Factory;

namespace TitheSync.Core.ViewModels
{
    public class ShellViewModel( IViewModelFactory viewModelFactory ):MvxViewModel
    {
        #region Fields

        private MvxViewModel? _currentMainContent;

        #endregion

        #region Properties

        public MvxViewModel? CurrentMainContent
        {
            get => _currentMainContent;
            private set => SetProperty(ref _currentMainContent, value);
        }

        #endregion

        #region Commands

        public IMvxCommand NavigateToHomeCommand => new MvxCommand(ExecuteNavigateToHome);
        public IMvxCommand NavigateToPaymentCommand => new MvxCommand(ExecuteNavigateToPayment);
        public IMvxCommand NavigateToDashboardCommand => new MvxCommand(ExecuteNavigateToDashboard);
        public IMvxCommand NavigateToNotificationCommand => new MvxCommand(ExecuteNavigateToNotification);
        public IMvxCommand NavigateToSettingsCommand => new MvxCommand(ExecuteNavigateToSettings);

        #endregion

        #region Methods

        private void ExecuteNavigateToSettings()
        {
            SettingsViewModel? settingsViewModel = viewModelFactory.CreateViewModel<SettingsViewModel>();
            CurrentMainContent = settingsViewModel;
            settingsViewModel?.Initialize();
        }

        private void ExecuteNavigateToNotification()
        {
            NotificationViewModel? notificationViewModel = viewModelFactory.CreateViewModel<NotificationViewModel>();
            CurrentMainContent = notificationViewModel;
            notificationViewModel?.Initialize();
        }

        private void ExecuteNavigateToDashboard()
        {
            DashboardViewModel? dashboardViewModel = viewModelFactory.CreateViewModel<DashboardViewModel>();
            CurrentMainContent = dashboardViewModel;
            dashboardViewModel?.Initialize();
        }

        private void ExecuteNavigateToPayment()
        {
            PaymentViewModel? paymentViewModel = viewModelFactory.CreateViewModel<PaymentViewModel>();
            CurrentMainContent = paymentViewModel;
            paymentViewModel?.Initialize();
        }

        private void ExecuteNavigateToHome()
        {
            HomeViewModel? homeViewmodel = viewModelFactory.CreateViewModel<HomeViewModel>();
            CurrentMainContent = homeViewmodel;
            homeViewmodel?.Initialize();
        }

        #endregion
    }
}
