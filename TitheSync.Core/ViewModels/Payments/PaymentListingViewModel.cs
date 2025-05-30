using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.Specialized;
using TitheSync.ApplicationState.Stores.Members;
using TitheSync.ApplicationState.Stores.Payments;
using TitheSync.Business.Services.Payments;
using TitheSync.Core.Controls;
using TitheSync.Core.Models;
using TitheSync.Core.Parameters;
using TitheSync.Domain.Models;
using TitheSync.Infrastructure.Services;

namespace TitheSync.Core.ViewModels.Payments
{
    public class PaymentListingViewModel:MvxViewModel, IPaymentListingViewModel
    {
        /// <summary>
        ///     Service for converting dates.
        /// </summary>
        private readonly IDateConverterService _dateConverterService;

        /// <summary>
        ///     Logger instance for logging messages and errors.
        /// </summary>
        private readonly ILogger<PaymentListingViewModel> _logger;

        private readonly IMemberStore _memberStore;

        private readonly IModalNavigationControl _modalNavigationControl;

        private readonly IPaymentService _paymentService;

        /// <summary>
        ///     Store for managing payment data.
        /// </summary>
        private readonly IPaymentStore _paymentStore;


        private MvxObservableCollection<AggregatedPayment> _aggregatedPayments;

        /// <summary>
        ///     A cancellation token source to manage cancellation of asynchronous operations.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        ///     Indicates whether the ViewModel is currently performing a loading operation.
        /// </summary>
        private bool _isLoading;

        /// <summary>
        ///     Backing field for the list of members, initialized from the member store.
        /// </summary>
        private MvxObservableCollection<PaymentWithName> _payments;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentListingViewModel" /> class.
        /// </summary>
        /// <param name="paymentService" >The payment service to load all payments from the repository.</param>
        /// <param name="paymentStore" >The payment store to manage payment data.</param>
        /// <param name="memberStore" >The member store to manage members</param>
        /// <param name="logger" >The logger instance for logging.</param>
        /// <param name="modalNavigationControl" >The control to popup modals</param>
        /// <param name="dateConverterService" >The service for converting dates</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when <paramref name="paymentStore" /> or <paramref name="logger" /> is
        ///     null.
        /// </exception>
        public PaymentListingViewModel( IPaymentService paymentService, IPaymentStore paymentStore, IMemberStore memberStore, ILogger<PaymentListingViewModel> logger,
            IModalNavigationControl modalNavigationControl,
            IDateConverterService dateConverterService )
        {
            try
            {
                _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
                _paymentStore = paymentStore ?? throw new ArgumentNullException(nameof(paymentStore));
                _memberStore = memberStore ?? throw new ArgumentNullException(nameof(memberStore));
                ;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _modalNavigationControl = modalNavigationControl ?? throw new ArgumentNullException(nameof(modalNavigationControl));
                _dateConverterService = dateConverterService ?? throw new ArgumentNullException(nameof(dateConverterService));

                _payments = new MvxObservableCollection<PaymentWithName>(_paymentStore.PaymentWithNames);

                _payments.CollectionChanged += PaymentsOnCollectionChanged;
                _paymentStore.OnPaymentAdded += PaymentStoreOnOnPaymentAdded;

                // Initialize commands
                OpenAddDialogAsyncCommand = new MvxCommand<int>(ExecuteOpenAddDialog);
                OpenDetailsDialogAsyncCommand = new MvxCommand<int>(ExecuteOpenDetailsDialog);


            }
            catch (ArgumentNullException e)
            {
                _logger?.LogError(e, "ArgumentNullException: {Message}", e.Message);
                throw;
            }
        }

        #region LifeCycle

        /// <summary>
        ///     Initializes the ViewModel asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        public override async Task Initialize()
        {
            _cancellationTokenSource?.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();
            await LoadPaymentsAsync(_cancellationTokenSource.Token);
            await base.Initialize();
        }


        /// <summary>
        ///     Cleans up resources and unsubscribes from events when the view is destroyed.
        /// </summary>
        /// <param name="viewFinishing" >
        ///     Indicates whether the view is finishing. Defaults to true.
        /// </param>
        public override void ViewDestroy( bool viewFinishing = true )
        {
            _payments.CollectionChanged -= PaymentsOnCollectionChanged;
            _paymentStore.OnPaymentAdded -= PaymentStoreOnOnPaymentAdded;
            base.ViewDestroy(viewFinishing);
        }

        /// <summary>
        ///     Handles the logic when the view is disappearing.
        ///     Cancels any ongoing operations and disposes of the cancellation token source.
        /// </summary>
        public override void ViewDisappearing()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            base.ViewDisappearing();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///     Event handler for when the payment collection changes.
        ///     Raises a property changed notification for the Payments property.
        /// </summary>
        /// <param name="sender" >The source of the event.</param>
        /// <param name="e" >The event data.</param>
        private void PaymentsOnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e ) => RaisePropertyChanged(() => Payments);

        /// <summary>
        ///     Event handler for when a new payment is added to the payment store.
        ///     Adds the new payment to the local-payments collection.
        /// </summary>
        /// <param name="payment" >The payment that was added.</param>
        private void PaymentStoreOnOnPaymentAdded( PaymentWithName payment ) => _payments.Add(payment);

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the ViewModel is currently performing a loading operation.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        ///     Publicly accessible collection of payments.
        /// </summary>
        public MvxObservableCollection<PaymentWithName> Payments
        {
            get => _payments;
            private set
            {
                _payments = value;
                RaisePropertyChanged(() => Payments);
            }
        }

        /// <summary>
        ///     Gets the count of payments in the collection.
        /// </summary>
        public int PaymentCount => Payments.Count;

        public MvxObservableCollection<AggregatedPayment> AggregatedPayments
        {
            get => _aggregatedPayments;
            private set
            {
                _aggregatedPayments = value;
                RaisePropertyChanged(() => AggregatedPayments);
            }
        }

        private string GetMonthName( int month )
        {
            return month switch
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

        #endregion

        #region Methods

        /// <summary>
        ///     Asynchronously loads the payment from the payment store.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadPaymentsAsync( CancellationToken cancellationToken )
        {
            if (IsLoading)
                return;
            IsLoading = true;
            try
            {
                // Simulate loading members
                // await Task.Delay(3000, _cancellationTokenSource.Token); // Example delay

                _payments.Clear();
                await _paymentService.GetPaymentsWithNamesAsync(cancellationToken);

                // Add loaded payments to the collection
                foreach (PaymentWithName payment in _paymentStore.PaymentWithNames)
                {
                    _payments.Add(payment);
                }

                UpdateAggregatedPayments();
                await RaisePropertyChanged(() => Payments);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("LoadPaymentsAsync operation was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payments.");
            }
            finally
            {
                IsLoading = false;
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
        private void ExecuteOpenAddDialog( int id )
        {
            NavigationParameter parameter = new()
            {
                Id = id,
                CallingViewModel = GetType()
            };
            _modalNavigationControl.PopUp<PaymentCreateFormViewModel>(parameter);
        }

        /// <summary>
        ///     Executes the logic to open the details dialog for a member.
        /// </summary>
        /// <param name="id" >The ID of the member.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private void ExecuteOpenDetailsDialog( int id ) => _modalNavigationControl.PopUp<PaymentDetailsFormViewModel>(id);


        private void UpdateAggregatedPayments()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            List<AggregatedPayment> groupedPayments = _payments
                                                      .Where(p => p.DatePaid.Month == currentMonth && p.DatePaid.Year == currentYear) // Filter by current month and year
                                                      .GroupBy(p => p.PaymentMemberId)
                                                      .Select(g => new AggregatedPayment
                                                      {
                                                          MemberId = g.Key,
                                                          MemberName = $"{g.First().FirstName} {g.First().LastName}",
                                                          TotalAmount = g.Sum(p => p.Amount),
                                                          CurrentMonth = GetMonthName(currentMonth)
                                                      })
                                                      .ToList();

            AggregatedPayments = new MvxObservableCollection<AggregatedPayment>(groupedPayments);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Command to open the dialog for adding a new payment.
        /// </summary>
        public IMvxCommand<int> OpenAddDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for viewing details of a member.
        /// </summary>
        public IMvxCommand<int> OpenDetailsDialogAsyncCommand { get; }

        #endregion
    }
}
