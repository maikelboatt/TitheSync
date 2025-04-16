using MvvmCross.ViewModels;
using TitheSync.Core.ViewModels.Payments;

namespace TitheSync.Core.ViewModels
{
    public class PaymentViewModel:MvxViewModel, IPaymentViewModel
    {
        public PaymentListingViewModel? PaymentListingViewModel { get; set; }
    }
}
