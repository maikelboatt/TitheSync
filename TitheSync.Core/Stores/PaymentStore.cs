using Microsoft.Extensions.Logging;
using TitheSync.Domain.Models;
using TitheSync.Domain.Services;

namespace TitheSync.Core.Stores
{
    /// <summary>
    ///     Represents a store for managing payments, providing thread-safe access and operations
    /// </summary>
    public class PaymentStore:IPaymentStore
    {
        private readonly ReaderWriterLockSlim _lock = new();
        private readonly ILogger<PaymentStore> _logger;
        private readonly List<Payment> _payments = [];
        private readonly IPaymentService _paymentService;
        private readonly List<PaymentWithName> _paymentsWithNames = [];

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentStore" /> class.
        /// </summary>
        /// <param name="paymentService" >The service used to manage payments</param>
        /// <param name="logger" >The logger instance for logging store operations.</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when the <paramref name="paymentService" /> and or
        ///     <seealso cref="logger" /> is null.
        /// </exception>
        public PaymentStore( IPaymentService paymentService, ILogger<PaymentStore> logger )
        {
            // Validate the payment service
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            // Initialize the logger
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Gets the list of payment in a thread-safe manner.
        /// </summary>
        public IEnumerable<Payment> Payments
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _payments.ToList();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
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
        ///     Loads payments asynchronously from the payment service.
        /// </summary>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task LoadPaymentAsync( CancellationToken cancellationToken = default )
        {
            try
            {
                IEnumerable<Payment> payments = await _paymentService.GetPaymentsAsync();
                _lock.EnterWriteLock();

                try
                {
                    _payments.Clear();
                    _payments.AddRange(payments);
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
        ///     Adds a payment asynchronously
        /// </summary>
        /// <param name="payment" >The payment to add</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task AddPaymentAsync( Payment payment, CancellationToken cancellationToken = default )
        {
            await _paymentService.AddPaymentAsync(payment);
            _lock.EnterWriteLock();
            try
            {
                _payments.Add(payment);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            OnPaymentAdded?.Invoke(payment);
        }

        /// <summary>
        ///     Updates an existing payment asynchronously
        /// </summary>
        /// <param name="payment" >The payment to update</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task UpdatePaymentAsync( Payment payment, CancellationToken cancellationToken = default )
        {
            await _paymentService.UpdatePaymentAsync(payment);
            _lock.EnterWriteLock();
            try
            {
                int index = _payments.FindIndex(p => p.PaymentId == payment.PaymentId);
                if (index >= 0)
                {
                    _payments[index] = payment;
                    OnPaymentUpdated?.Invoke(payment);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Deletes a payment asynchronously by its id
        /// </summary>
        /// <param name="paymentId" >The Id of the payment to delete</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task DeletePaymentAsync( int paymentId, CancellationToken cancellationToken = default )
        {
            await _paymentService.DeletePaymentAsync(paymentId);

            _lock.EnterWriteLock();
            try
            {
                Payment? payment = _payments.FirstOrDefault(p => p.PaymentId == paymentId);
                if (payment != null)
                {
                    _payments.Remove(payment);
                    OnPaymentDeleted?.Invoke(payment);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
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
        public event Action<Payment>? OnPaymentUpdated;

        /// <summary>
        ///     Event triggered when payment is deleted.
        /// </summary>
        public event Action<Payment>? OnPaymentDeleted;

        /// <summary>
        ///     Event triggered when payments are loaded.
        /// </summary>
        public event Action? OnPaymentsLoaded;

        /// <summary>
        ///     Loads payments with names asynchronously from the payment service.
        /// </summary>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task LoadPaymentWithNamesAsync( CancellationToken cancellationToken = default )
        {
            try
            {
                IEnumerable<PaymentWithName> payments = await _paymentService.GetPaymentsWithNamesAsync();
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payments with names");
                throw;
            }
        }
    }
}
