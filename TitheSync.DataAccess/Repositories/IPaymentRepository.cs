using TitheSync.Domain.Models;

namespace TitheSync.DataAccess.Repositories
{
    /// <summary>
    ///     Interface for managing payment-related operations in the repository.
    /// </summary>
    public interface IPaymentRepository
    {
        /// <summary>
        ///     Retrieves all payments asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of payments.</returns>
        Task<IEnumerable<Payment>> GetPaymentsAsync();

        /// <summary>
        ///     Retrieves all payments with member names asynchronously.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains a collection of payments with
        ///     names.
        /// </returns>
        Task<IEnumerable<PaymentWithName>> GetPaymentsWithNamesAsync();

        /// <summary>
        ///     Retrieves a payment by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id" >The unique identifier of the payment.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the payment with the specified ID.</returns>
        Task<Payment?> GetPaymentByIdAsync( int id );

        /// <summary>
        ///     Adds a new payment to the repository asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<int> AddPaymentAsync( PaymentWithName payment );

        /// <summary>
        ///     Updates an existing payment in the repository asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdatePaymentAsync( PaymentWithName payment );

        /// <summary>
        ///     Deletes a payment from the repository by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id" >The unique identifier of the payment to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeletePaymentAsync( int id );
    }
}
