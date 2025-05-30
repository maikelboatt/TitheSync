using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Payments
{
    /// <summary>
    ///     Defines the contract for payment-related operations, including retrieval,
    ///     addition, update, and deletion of payments, as well as fetching payments with member names.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        ///     Retrieves all payments with associated member names asynchronously.
        /// </summary>
        /// <param name="cancellationToken" >A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task GetPaymentsWithNamesAsync( CancellationToken cancellationToken = default );

        /// <summary>
        ///     Retrieves a payment by its unique identifier asynchronously.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to retrieve.</param>
        /// <returns>The payment with the specified ID, or null if not found.</returns>
        PaymentWithName? GetPaymentByIdAsync( int paymentId );

        /// <summary>
        ///     Retrieves all payments associated with a specific member.
        /// </summary>
        /// <param name="memberId" >The ID of the member whose payments to retrieve.</param>
        /// <returns>A collection of payments with member names.</returns>
        IEnumerable<PaymentWithName> GetPaymentsByMemberId( int memberId );

        /// <summary>
        ///     Adds a new payment asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        /// <param name="cancellationToken" >A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddPaymentAsync( PaymentWithName payment, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Updates an existing payment asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        /// <param name="cancellationToken" >A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdatePaymentAsync( PaymentWithName payment, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Deletes a payment by its unique identifier asynchronously.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to delete.</param>
        /// <param name="cancellationToken" >A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeletePaymentAsync( int paymentId, CancellationToken cancellationToken = default );
    }
}
