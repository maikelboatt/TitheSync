using TitheSync.ApplicationState.Stores.Payments;
using TitheSync.DataAccess.Repositories;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Payments
{
    /// <summary>
    ///     Service for managing payments, including CRUD operations and state synchronization
    ///     between repositories and application state stores.
    /// </summary>
    public class PaymentService:IPaymentService
    {
        /// <summary>
        ///     Repository for accessing payment data.
        /// </summary>
        private readonly IPaymentRepository _paymentRepository;

        /// <summary>
        ///     Store for managing payment state in the application.
        /// </summary>
        private readonly IPaymentStore _paymentStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentService" /> class.
        /// </summary>
        /// <param name="paymentRepository" >The payment repository for data access.</param>
        /// <param name="paymentStore" >The payment store for application state management.</param>
        public PaymentService(
            IPaymentRepository paymentRepository,
            IPaymentStore paymentStore )
        {
            _paymentRepository = paymentRepository;
            _paymentStore = paymentStore;
        }

        /// <summary>
        ///     Retrieves all payments with member names asynchronously from the repository,
        ///     updates the payment store, and refreshes the member store.
        /// </summary>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task GetPaymentsWithNamesAsync( CancellationToken cancellationToken = default )
        {
            IEnumerable<PaymentWithName> payments = await _paymentRepository.GetPaymentsWithNamesAsync();
            _paymentStore.GetPaymentsWithNames(payments, cancellationToken);
        }

        /// <summary>
        ///     Retrieves a payment by its ID from the payment store.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to retrieve.</param>
        /// <returns>The payment with the specified ID, or null if not found.</returns>
        public PaymentWithName? GetPaymentByIdAsync( int paymentId ) => _paymentStore.GetPaymentById(paymentId);

        /// <summary>
        ///     Returns the name of the month corresponding to the given month number.
        /// </summary>
        /// <param name="monthNumber" >The number of the month (1 for January, 12 for December).</param>
        /// <returns>The name of the month, or an empty string if the number is invalid.</returns>
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
        public IEnumerable<PaymentWithName> GetPaymentsByMemberId( int memberId, CancellationToken cancellationToken ) => _paymentStore.GetPaymentsByMemberId(memberId, cancellationToken);

        /// <summary>
        ///     Adds a new payment asynchronously to the repository and updates the payment store.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddPaymentAsync( PaymentWithName payment, CancellationToken cancellationToken = default )
        {
            int uniqueId = await _paymentRepository.AddPaymentAsync(payment);
            PaymentWithName paymentWithCorrectId = _paymentStore.CreatePaymentWithCorrectPaymentId(uniqueId, payment);
            _paymentStore.AddPayment(paymentWithCorrectId, cancellationToken);
        }

        /// <summary>
        ///     Updates an existing payment asynchronously in the repository and updates the payment store.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdatePaymentAsync( PaymentWithName payment, CancellationToken cancellationToken = default )
        {
            await _paymentRepository.UpdatePaymentAsync(payment);
            _paymentStore.UpdatePayment(payment, cancellationToken);
        }

        /// <summary>
        ///     Deletes a payment by its ID asynchronously from the repository and updates the payment store.
        /// </summary>
        /// <param name="paymentId" >The ID of the payment to delete.</param>
        /// <param name="cancellationToken" >A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeletePaymentAsync( int paymentId, CancellationToken cancellationToken = default )
        {
            await _paymentRepository.DeletePaymentAsync(paymentId);
            _paymentStore.DeletePayment(paymentId, cancellationToken);
        }
    }
}
