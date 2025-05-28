using MvvmCross.ViewModels;
using TitheSync.Domain.Models;

namespace TitheSync.Core.ViewModels.Payments
{
    public class BatchPaymentFormViewModel:MvxViewModel<Member>, IBatchPaymentFormViewModel
    {
        public override void Prepare( Member parameter )
        {
            throw new NotImplementedException();
        }
    }
}
