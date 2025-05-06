using TitheSync.Domain.Models;

namespace TitheSync.Core.Stores
{
    public interface IPaymentStore
    {
        /// <summary>
        ///     Gets the collection of payments.
        /// </summary>
        IEnumerable<Payment> Payments { get; }

        /// <summary>
        ///     Loads the collection of payments asynchronously.
        /// </summary>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task LoadPaymentAsync( CancellationToken cancellationToken = default );

        /// <summary>
        ///     Adds a new payment to the collection asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task AddPaymentAsync( Payment payment, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Updates an existing payment in the collection asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task UpdatePaymentAsync( Payment payment, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Deletes a payment from the collection asynchronously by their ID.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to delete.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task DeletePaymentAsync( int paymentId, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Event triggered when payments are changed.
        /// </summary>
        event Action<Payment> OnPaymentsChanged;

        /// <summary>
        ///     Event triggered when a payment is added.
        /// </summary>
        event Action<Payment> OnPaymentAdded;

        /// <summary>
        ///     Event triggered when a payment is updated.
        /// </summary>
        event Action<Payment> OnPaymentUpdated;

        /// <summary>
        ///     Event triggered when a payment is deleted.
        /// </summary>
        event Action<Payment> OnPaymentDeleted;

        /// <summary>
        ///     Event triggered when payments are loaded.
        /// </summary>
        event Action OnPaymentsLoaded;
    }
}
