using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using TitheSync.Core.Stores;
using TitheSync.Domain.Models;

namespace TitheSync.Core.ViewModels.Payments
{
    public class PaymentListingViewModel:MvxViewModel, IPaymentListingViewModel
    {
        /// <summary>
        ///     A cancellation token source to manage cancellation of asynchronous operations.
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        ///     Logger instance for logging messages and errors.
        /// </summary>
        private readonly ILogger<PaymentListingViewModel> _logger;

        /// <summary>
        ///     Store for managing payment data.
        /// </summary>
        private readonly IPaymentStore _paymentStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentListingViewModel" /> class.
        /// </summary>
        /// <param name="paymentStore" >The payment store to manage payment data.</param>
        /// <param name="logger" >The logger instance for logging.</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when <paramref name="paymentStore" /> or <paramref name="logger" /> is
        ///     null.
        /// </exception>
        public PaymentListingViewModel( IPaymentStore paymentStore, ILogger<PaymentListingViewModel> logger )
        {
            try
            {
                _paymentStore = paymentStore ?? throw new ArgumentNullException(nameof(paymentStore));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                _payments.CollectionChanged += ( sender, args ) => RaisePropertyChanged(nameof(Payments));

                // Initialize commands
                OpenAddDialogAsyncCommand = new MvxAsyncCommand(ExecuteOpenAddDialog);
                OpenUpdateDialogAsyncCommand = new MvxAsyncCommand<int>(ExecuteOpenUpdateDialog);
                OpenDeleteDialogAsyncCommand = new MvxAsyncCommand<int>(ExecuteOpenDeleteDialog);
            }
            catch (ArgumentNullException e)
            {
                _logger?.LogError(e, "ArgumentNullException: {Message}", e.Message);
                throw;
            }
        }

        /// <summary>
        ///     Collection of payments, backed by the payment store.
        /// </summary>
        private MvxObservableCollection<Payment> _payments => new(_paymentStore.Payments.ToList());

        /// <summary>
        ///     Publicly accessible collection of payments.
        /// </summary>
        public IEnumerable<Payment> Payments => _payments;

        /// <summary>
        ///     Initializes the view model asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        public override async Task Initialize()
        {
            // Initialize the base class
            await base.Initialize();
            await LoadPaymentsAsync();
            // await CreatePaymentRecord();
        }

        #region Methods

        /// <summary>
        ///     Asynchronously loads the payment from the payment store.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadPaymentsAsync()
        {
            try
            {
                await _paymentStore.LoadPaymentAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("LoadPaymentsAsync operation was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payments.");
            }
        }

        /// <summary>
        ///     Cancels the ongoing loading operation for payment.
        /// </summary>
        private void CancelLoading() => _cancellationTokenSource.Cancel();

        /// <summary>
        ///     Executes the logic to open the dialog for adding a new payment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task ExecuteOpenAddDialog() => throw
            // _modalNavigationControl.PopUp<RemovalEventCreateFormViewModel>(AnimalId); // Pass AnimalId to Create form
            new NotImplementedException();

        /// <summary>
        ///     Executes the logic to open the dialog for updating an existing payment.
        /// </summary>
        /// <param name="id" >The ID of the payment to update.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task ExecuteOpenUpdateDialog( int id, CancellationToken cancellationToken = default ) => throw new NotImplementedException();

        /// <summary>
        ///     Executes the logic to open the dialog for deleting an existing payment.
        /// </summary>
        /// <param name="id" >The ID of the payment to delete.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task ExecuteOpenDeleteDialog( int id, CancellationToken cancellationToken = default ) => throw new NotImplementedException();

        #endregion

        #region Commands

        /// <summary>
        ///     Command to open the dialog for adding a new payment.
        /// </summary>
        public IMvxAsyncCommand OpenAddDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for updating an existing payment.
        /// </summary>
        public IMvxAsyncCommand<int> OpenUpdateDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for deleting an existing payment.
        /// </summary>
        public IMvxAsyncCommand<int> OpenDeleteDialogAsyncCommand { get; }

        #endregion
    }
}
