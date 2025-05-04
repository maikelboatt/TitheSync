using MvvmCross.ViewModels;
using TitheSync.Core.Factory;
using TitheSync.Core.ViewModels.Payments;

namespace TitheSync.Core.ViewModels
{
    public class PaymentViewModel( IViewModelFactory viewModelFactory ):MvxViewModel, IPaymentViewModel
    {
        public PaymentListingViewModel? PaymentListingViewModel => CreateViewModel();

        private PaymentListingViewModel? CreateViewModel()
        {
            PaymentListingViewModel? viewModel = viewModelFactory.CreateViewModel<PaymentListingViewModel>();
            viewModel?.Initialize();
            return viewModel;
        }
    }
}
