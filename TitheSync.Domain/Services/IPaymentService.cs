using TitheSync.Domain.Models;

namespace TitheSync.Domain.Services
{
    public interface IPaymentService
    {
        /// <summary>
        ///     Retrieves all payments asynchronously.
        /// </summary>
        /// <returns>A collection of payments.</returns>
        Task<IEnumerable<Payment>> GetPaymentsAsync();

        /// <summary>
        ///     Retrieves a payment by its ID asynchronously.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to retrieve.</param>
        /// <returns>The payment with the specified ID.</returns>
        Task<Payment> GetPaymentByIdAsync( int paymentId );

        /// <summary>
        ///     Adds a new payment asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        Task AddPaymentAsync( Payment payment );

        /// <summary>
        ///     Updates an existing payment asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        Task UpdatePaymentAsync( Payment payment );

        /// <summary>
        ///     Deletes a payment by its ID asynchronously.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to delete.</param>
        Task DeletePaymentAsync( int paymentId );
    }
}
