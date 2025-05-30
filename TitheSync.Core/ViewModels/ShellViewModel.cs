using MvvmCross.Commands;
using MvvmCross.ViewModels;
using TitheSync.ApplicationState.Stores;
using TitheSync.Core.Factory;

namespace TitheSync.Core.ViewModels
{
    public class ShellViewModel:MvxViewModel
    {
        public ShellViewModel( IViewModelFactory viewModelFactory, IModalNavigationStore modalNavigationStore )
        {
            _viewModelFactory = viewModelFactory;
            _modalNavigationStore = modalNavigationStore;

            // Initialize the main content to DashboardViewModel
            DashboardViewModel? dashboardViewModel = _viewModelFactory.CreateViewModel<DashboardViewModel>();
            CurrentMainContent = dashboardViewModel;

            // Initialize commands
            NavigateToHomeCommand = new MvxCommand(ExecuteNavigateToHome);
            NavigateToPaymentCommand = new MvxCommand(ExecuteNavigateToPayment);
            NavigateToDashboardCommand = new MvxCommand(ExecuteNavigateToDashboard);
            NavigateToNotificationCommand = new MvxCommand(ExecuteNavigateToNotification);
            NavigateToSettingsCommand = new MvxCommand(ExecuteNavigateToSettings);

            modalNavigationStore.CurrentModalViewModelChanged += ModalNavigationStoreOnCurrentModalViewModelChanged;
        }

        private void ModalNavigationStoreOnCurrentModalViewModelChanged()
        {
            RaisePropertyChanged(nameof(CurrentModalContent));
            RaisePropertyChanged(nameof(IsModalOpen));
        }

        #region Properties

        public MvxViewModel? CurrentMainContent
        {
            get => _currentMainContent;
            private set => SetProperty(ref _currentMainContent, value);
        }

        public bool IsModalOpen => _modalNavigationStore.IsOpen;
        public MvxViewModel? CurrentModalContent => _modalNavigationStore.CurrentModalViewModel;

        #endregion

        #region Fields

        private MvxViewModel? _currentMainContent;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IModalNavigationStore _modalNavigationStore;

        #endregion

        #region Commands

        public IMvxCommand NavigateToHomeCommand { get; }
        public IMvxCommand NavigateToPaymentCommand { get; }
        public IMvxCommand NavigateToDashboardCommand { get; }
        public IMvxCommand NavigateToNotificationCommand { get; }
        public IMvxCommand NavigateToSettingsCommand { get; }

        #endregion

        #region Methods

        private void ExecuteNavigateToSettings()
        {
            SettingsViewModel? settingsViewModel = _viewModelFactory.CreateViewModel<SettingsViewModel>();
            CurrentMainContent = settingsViewModel;
            settingsViewModel?.Initialize();
        }

        private void ExecuteNavigateToNotification()
        {
            NotificationViewModel? notificationViewModel = _viewModelFactory.CreateViewModel<NotificationViewModel>();
            CurrentMainContent = notificationViewModel;
            notificationViewModel?.Initialize();
        }

        private void ExecuteNavigateToDashboard()
        {
            DashboardViewModel? dashboardViewModel = _viewModelFactory.CreateViewModel<DashboardViewModel>();
            CurrentMainContent = dashboardViewModel;
            dashboardViewModel?.Initialize();
        }

        private void ExecuteNavigateToPayment()
        {
            PaymentViewModel? paymentViewModel = _viewModelFactory.CreateViewModel<PaymentViewModel>();
            CurrentMainContent = paymentViewModel;
            paymentViewModel?.Initialize();
        }

        private void ExecuteNavigateToHome()
        {
            HomeViewModel? homeViewmodel = _viewModelFactory.CreateViewModel<HomeViewModel>();
            CurrentMainContent = homeViewmodel;
            homeViewmodel?.Initialize();
        }

        #endregion
    }
}
