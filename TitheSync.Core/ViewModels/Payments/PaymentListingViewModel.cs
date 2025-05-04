using MvvmCross.ViewModels;
using TitheSync.Domain.Models;
using TitheSync.Domain.Services;

namespace TitheSync.Core.ViewModels.Payments
{
    public class PaymentListingViewModel( IPaymentService paymentService ):MvxViewModel
    {
        public override async Task Initialize()
        {
            // Initialize the base class
            await base.Initialize();
            await LoadAllPayments();
            // await CreatePaymentRecord();
        }

        private async Task CreatePaymentRecord()
        {
            Payment payment = new(
                0,
                1, // Assuming a member with ID 1 exists
                100.00m,
                new DateOnly(2025, 04, 07)
            );

            await paymentService.AddPaymentAsync(payment);
        }

        private async Task LoadAllPayments()
        {
            IEnumerable<Payment> result = await paymentService.GetPaymentsAsync();
        }
    }
}
