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
        ///     Gets a read-only list of payments with associated names.
        /// </summary>
        IReadOnlyList<PaymentWithName> PaymentWithNames { get; }

        /// <summary>
        ///     Gets all payments made by a specific member.
        /// </summary>
        IReadOnlyList<PaymentWithName> PaymentByMember { get; }

        /// <summary>
        ///     Loads the collection of payments asynchronously.
        /// </summary>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task LoadPaymentAsync( CancellationToken cancellationToken = default );

        /// <summary>
        ///     Loads the collection of payments asynchronously.
        /// </summary>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task LoadPaymentWithNamesAsync( CancellationToken cancellationToken = default );

        /// <summary>
        ///     Adds a new payment to the collection asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task AddPaymentAsync( PaymentWithName payment, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Updates an existing payment in the collection asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task UpdatePaymentAsync( PaymentWithName payment, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Deletes a payment from the collection asynchronously by their ID.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to delete.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task DeletePaymentAsync( int paymentId, CancellationToken cancellationToken = default );


        /// <summary>
        ///     Retrieves the member associated with a specific payment ID.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to retrieve the member for.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        /// <returns>The member associated with the specified payment ID, or null if not found.</returns>
        PaymentWithName? GetMemberByPaymentId( int paymentId, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Retrieves all payments associated with a specific member ID asynchronously.
        /// </summary>
        /// <param name="memberId" >The ID of the member whose payments are to be retrieved.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.The task result contains a collection of
        ///     <see cref="PaymentWithName" /> objects associated with the specified member ID.
        /// </returns>
        IEnumerable<PaymentWithName> GetPaymentsByMemberId( int memberId );

        /// <summary>
        ///     Event triggered when payments are changed.
        /// </summary>
        event Action<Payment> OnPaymentsChanged;

        /// <summary>
        ///     Event triggered when a payment is added.
        /// </summary>
        event Action<PaymentWithName> OnPaymentAdded;

        /// <summary>
        ///     Event triggered when a payment is updated.
        /// </summary>
        event Action<PaymentWithName> OnPaymentUpdated;

        /// <summary>
        ///     Event triggered when a payment is deleted.
        /// </summary>
        event Action<PaymentWithName> OnPaymentDeleted;

        /// <summary>
        ///     Event triggered when payments are loaded.
        /// </summary>
        event Action OnPaymentsLoaded;
    }
}
