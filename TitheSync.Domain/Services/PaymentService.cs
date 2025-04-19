using TitheSync.Domain.Models;
using TitheSync.Domain.Repositories;

namespace TitheSync.Domain.Services
{
    /// <summary>
    ///     Service for managing payments.
    /// </summary>
    public class PaymentService( IPaymentRepository paymentRepository ):IPaymentService
    {
        /// <summary>
        ///     Retrieves all payments asynchronously.
        /// </summary>
        /// <returns>A collection of payments.</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsAsync() => await paymentRepository.GetPaymentsAsync();

        /// <summary>
        ///     Retrieves a payment by its ID asynchronously.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to retrieve.</param>
        /// <returns>The payment with the specified ID.</returns>
        public async Task<Payment> GetPaymentByIdAsync( int paymentId ) => await paymentRepository.GetPaymentByIdAsync(paymentId);

        /// <summary>
        ///     Adds a new payment asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        public async Task AddPaymentAsync( Payment payment )
        {
            await paymentRepository.AddPaymentAsync(payment);
        }

        /// <summary>
        ///     Updates an existing payment asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        public async Task UpdatePaymentAsync( Payment payment )
        {
            await paymentRepository.UpdatePaymentAsync(payment);
        }

        /// <summary>
        ///     Deletes a payment by its ID asynchronously.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to delete.</param>
        public async Task DeletePaymentAsync( int paymentId )
        {
            await paymentRepository.DeletePaymentAsync(paymentId);
        }
    }
}
