using Microsoft.Extensions.Logging;
using TitheSync.Domain.Models;

namespace TitheSync.ApplicationState.Stores.Payments
{
    /// <summary>
    ///     Represents a store for managing payments, providing thread-safe access and operations.
    /// </summary>
    public class PaymentStore:IPaymentStore
    {
        /// <summary>
        ///     Lock for thread-safe access to payment collections.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new();

        /// <summary>
        ///     Logger instance for logging store operations.
        /// </summary>
        private readonly ILogger<PaymentStore> _logger;

        private readonly List<PaymentWithName> _paymentsWithNames = [];

        // /// <summary>
        // ///     List of payments with member names.
        // /// </summary>
        // private readonly List<PaymentWithName> _paymentsWithNames =
        // [
        //     new(1, 101, 100.50m, new DateOnly(2023, 1, 1), "John", "Doe"),
        //     new(2, 102, 200.75m, new DateOnly(2023, 1, 2), "Jane", "Smith"),
        //     new(3, 103, 150.00m, new DateOnly(2023, 1, 3), "Michael", "Brown"),
        //     new(4, 104, 300.25m, new DateOnly(2023, 1, 4), "Emily", "Davis"),
        //     new(5, 105, 250.00m, new DateOnly(2023, 1, 5), "Chris", "Wilson"),
        //     new(6, 106, 175.50m, new DateOnly(2023, 1, 6), "Sarah", "Taylor"),
        //     new(7, 107, 225.75m, new DateOnly(2023, 1, 7), "David", "Anderson"),
        //     new(8, 108, 125.00m, new DateOnly(2023, 1, 8), "Laura", "Thomas"),
        //     new(9, 109, 275.25m, new DateOnly(2023, 1, 9), "James", "Moore"),
        //     new(10, 110, 350.00m, new DateOnly(2023, 1, 10), "Anna", "Jackson"),
        //     new(11, 111, 400.50m, new DateOnly(2023, 1, 11), "Robert", "White"),
        //     new(12, 112, 450.75m, new DateOnly(2023, 1, 12), "Sophia", "Harris"),
        //     new(13, 113, 500.00m, new DateOnly(2023, 1, 13), "Daniel", "Martin"),
        //     new(14, 114, 550.25m, new DateOnly(2023, 1, 14), "Olivia", "Thompson"),
        //     new(15, 115, 600.00m, new DateOnly(2023, 1, 15), "Matthew", "Garcia"),
        //     new(16, 116, 650.50m, new DateOnly(2023, 1, 16), "Isabella", "Martinez"),
        //     new(17, 117, 700.75m, new DateOnly(2023, 1, 17), "Andrew", "Robinson"),
        //     new(18, 118, 750.00m, new DateOnly(2023, 1, 18), "Mia", "Clark"),
        //     new(19, 119, 800.25m, new DateOnly(2023, 1, 19), "Joshua", "Rodriguez"),
        //     new(20, 120, 850.00m, new DateOnly(2023, 1, 20), "Emma", "Lewis")
        // ];

        // Todo: Set paymentsWithNames field to an empty collection

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentStore" /> class.
        /// </summary>
        /// <param name="logger" >The logger instance for logging store operations.</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when the <paramref name="logger" /> is null.
        /// </exception>
        public PaymentStore( ILogger<PaymentStore> logger )
        {
            // Initialize the logger
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Gets the list of payments with names in a thread-safe manner.
        /// </summary>
        public IReadOnlyList<PaymentWithName> PaymentWithNames
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _paymentsWithNames.ToList();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }


        /// <summary>
        ///     Event triggered when payments are changed.
        /// </summary>
        public event Action<Payment>? OnPaymentsChanged;

        /// <summary>
        ///     Event triggered when payment is added.
        /// </summary>
        public event Action<PaymentWithName>? OnPaymentAdded;

        /// <summary>
        ///     Event triggered when payment is updated.
        /// </summary>
        public event Action<PaymentWithName>? OnPaymentUpdated;

        /// <summary>
        ///     Event triggered when payment is deleted.
        /// </summary>
        public event Action<PaymentWithName>? OnPaymentDeleted;

        /// <summary>
        ///     Event triggered when payments are loaded.
        /// </summary>
        public event Action? OnPaymentsLoaded;

        /// <summary>
        ///     Loads payments asynchronously from the payment service.
        /// </summary>
        /// <param name="payments" >The payments to load.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public void GetPaymentsWithNames( IEnumerable<PaymentWithName> payments, CancellationToken cancellationToken = default )
        {
            try
            {
                _lock.EnterWriteLock();

                try
                {
                    _paymentsWithNames.Clear();
                    _paymentsWithNames.AddRange(payments);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
                OnPaymentsLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payments");
                throw;
            }
        }

        /// <summary>
        ///     Retrieves the member associated with a particular payment ID.
        /// </summary>
        /// <param name="paymentId" >The id of the payment to retrieve the member for.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        /// <returns>The member associated with the specified payment id, or null if not found.</returns>
        public PaymentWithName? GetPaymentById( int paymentId, CancellationToken cancellationToken = default )
        {
            return _paymentsWithNames.FirstOrDefault(p => p.PaymentId == paymentId);
        }

        /// <summary>
        ///     Retrieves all payments associated with a specific member ID asynchronously.
        /// </summary>
        /// <param name="memberId" >The ID of the member whose payments are to be retrieved.</param>
        /// <param name="cancellationToken" ></param>
        /// <returns>
        ///     A collection of <see cref="PaymentWithName" /> objects associated with the specified member ID.
        /// </returns>
        public IEnumerable<PaymentWithName> GetPaymentsByMemberId( int memberId, CancellationToken cancellationToken = default )
        {
            return
            [
                .._paymentsWithNames
                    .Where(p => p.PaymentMemberId == memberId)
            ];
        }

        /// <summary>
        ///     Adds a payment asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public void AddPayment( PaymentWithName payment, CancellationToken cancellationToken = default )
        {
            _lock.EnterWriteLock();
            try
            {
                _paymentsWithNames.Add(payment);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            OnPaymentAdded?.Invoke(payment);
        }


        /// <summary>
        ///     Updates an existing payment asynchronously.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public void UpdatePayment( PaymentWithName payment, CancellationToken cancellationToken = default )
        {
            _lock.EnterWriteLock();
            try
            {
                int index = _paymentsWithNames.FindIndex(p => p.PaymentId == payment.PaymentId);
                if (index >= 0)
                {
                    _paymentsWithNames[index] = payment;
                    OnPaymentUpdated?.Invoke(payment);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Deletes a payment asynchronously by its id.
        /// </summary>
        /// <param name="paymentId" >The Id of the payment to delete.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public void DeletePayment( int paymentId, CancellationToken cancellationToken = default )
        {
            _lock.EnterWriteLock();
            try
            {
                PaymentWithName? payment = _paymentsWithNames.FirstOrDefault(p => p.PaymentId == paymentId);
                if (payment != null)
                {
                    _paymentsWithNames.Remove(payment);
                    OnPaymentDeleted?.Invoke(payment);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
