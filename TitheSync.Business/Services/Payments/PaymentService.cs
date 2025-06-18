using TitheSync.ApplicationState.Stores.Payments;
using TitheSync.Business.Services.Errors;
using TitheSync.DataAccess.Repositories;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Payments
{
    /// <summary>
    ///     Service for managing payment operations, including retrieval, addition, update, and deletion of payments.
    /// </summary>
    /// <remarks>
    ///     This service interacts with the payment repository for data access and the payment store for state management.
    ///     It also handles database errors using the provided error handler service.
    /// </remarks>
    public class PaymentService( IPaymentRepository paymentRepository, IPaymentStore paymentStore, IDatabaseErrorHandlerService databaseErrorHandlerService ):IPaymentService
    {
        /// <summary>
        ///     Retrieves all payments with associated names from the repository and updates the payment store.
        /// </summary>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        public async Task GetPaymentsWithNamesAsync( CancellationToken cancellationToken = default )
        {
            IEnumerable<PaymentWithName> paymentsWithNames = await databaseErrorHandlerService.HandleDatabaseOperationAsync(
                paymentRepository.GetPaymentsWithNamesAsync,
                "Retrieving payments with names"
            ) ?? [];
            paymentStore.GetPaymentsWithNames(paymentsWithNames, cancellationToken);
        }

        /// <summary>
        ///     Retrieves a payment by its ID from the payment store.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to retrieve.</param>
        /// <returns>The payment with the specified ID, or null if not found.</returns>
        public PaymentWithName? GetPaymentByIdAsync( int paymentId ) => paymentStore.GetPaymentById(paymentId);

        /// <summary>
        ///     Returns the name of the month corresponding to the given month number.
        /// </summary>
        /// <param name="monthNumber" >The number of the month (1 for January, 12 for December).</param>
        /// <returns>The name of the month, or "Invalid Month" if the number is invalid.</returns>
        public string GetMonthName( int monthNumber )
        {
            return monthNumber switch
            {
                1  => "January",
                2  => "February",
                3  => "March",
                4  => "April",
                5  => "May",
                6  => "June",
                7  => "July",
                8  => "August",
                9  => "September",
                10 => "October",
                11 => "November",
                12 => "December",
                _  => "Invalid Month"
            };
        }

        /// <summary>
        ///     Retrieves all payments for a specific member from the payment store.
        /// </summary>
        /// <param name="memberId" >The ID of the member whose payments to retrieve.</param>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A collection of payments with names for the specified member.</returns>
        public IEnumerable<PaymentWithName> GetPaymentsByMemberId( int memberId, CancellationToken cancellationToken ) => paymentStore.GetPaymentsByMemberId(memberId, cancellationToken);

        /// <summary>
        ///     Adds a new payment to the repository and updates the payment store with the correct payment ID.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        public async Task AddPaymentAsync( PaymentWithName payment, CancellationToken cancellationToken = default )
        {
            int? uniqueId = await databaseErrorHandlerService.HandleDatabaseOperationAsync(
                () => paymentRepository.AddPaymentAsync(payment),
                "Adding payments"
            );
            {
                PaymentWithName paymentWithCorrectId = paymentStore.CreatePaymentWithCorrectPaymentId(uniqueId.Value, payment);
                paymentStore.AddPayment(paymentWithCorrectId, cancellationToken);
            }
        }

        /// <summary>
        ///     Updates an existing payment in the repository and updates the payment store.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        public async Task UpdatePaymentAsync( PaymentWithName payment, CancellationToken cancellationToken = default )
        {
            await databaseErrorHandlerService.HandleDatabaseOperationAsync(
                () => paymentRepository.UpdatePaymentAsync(payment),
                "Updating payments"
            );
            paymentStore.UpdatePayment(payment, cancellationToken);
        }

        /// <summary>
        ///     Deletes a payment from the repository and removes it from the payment store.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to delete.</param>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        public async Task DeletePaymentAsync( int paymentId, CancellationToken cancellationToken = default )
        {
            await databaseErrorHandlerService.HandleDatabaseOperationAsync(
                () => paymentRepository.DeletePaymentAsync(paymentId),
                "Deleting payments"
            );
            paymentStore.DeletePayment(paymentId, cancellationToken);
        }
    }
}
