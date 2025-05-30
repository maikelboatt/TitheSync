using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.Specialized;
using TitheSync.ApplicationState.Stores;
using TitheSync.ApplicationState.Stores.Payments;
using TitheSync.Business.Services.Members;
using TitheSync.Business.Services.Payments;
using TitheSync.Core.Controls;
using TitheSync.Core.Parameters;
using TitheSync.Domain.Models;

namespace TitheSync.Core.ViewModels.Payments
{
    /// <summary>
    ///     ViewModel for displaying and managing payment details for a specific member.
    ///     Handles loading, updating, adding, and deleting payments, as well as dialog navigation.
    /// </summary>
    public class PaymentDetailsFormViewModel:MvxViewModel<int>, IPaymentDetailsViewModel
    {
        /// <summary>
        ///     Logger for diagnostic and error messages.
        /// </summary>
        private readonly ILogger<PaymentDetailsFormViewModel> _logger;

        /// <summary>
        ///     Service for member-related operations.
        /// </summary>
        private readonly IMemberService _memberService;

        /// <summary>
        ///     Control for modal dialog navigation.
        /// </summary>
        private readonly IModalNavigationControl _modalNavigationControl;

        /// <summary>
        ///     Store for managing modal navigation state.
        /// </summary>
        private readonly IModalNavigationStore _modalNavigationStore;

        /// <summary>
        ///     Service for payment-related operations.
        /// </summary>
        private readonly IPaymentService _paymentService;

        /// <summary>
        ///     Store for managing payment state and events.
        /// </summary>
        private readonly IPaymentStore _paymentStore;

        /// <summary>
        ///     A cancellation token source to manage cancellation of asynchronous operations.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        ///     The ID of the member whose payments are being managed.
        /// </summary>
        private int _memberId;

        /// <summary>
        ///     Collection of payments associated with the member.
        /// </summary>
        private MvxObservableCollection<PaymentWithName> _payments = [];

        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentDetailsFormViewModel" /> class.
        /// </summary>
        /// <param name="logger" >Logger instance.</param>
        /// <param name="paymentService" >Payment service instance.</param>
        /// <param name="memberService" >Member service instance.</param>
        /// <param name="paymentStore" >Payment store instance.</param>
        /// <param name="modalNavigationControl" >Modal navigation control instance.</param>
        /// <param name="modalNavigationStore" >Modal navigation store instance.</param>
        public PaymentDetailsFormViewModel(
            ILogger<PaymentDetailsFormViewModel> logger,
            IPaymentService paymentService,
            IMemberService memberService,
            IPaymentStore paymentStore,
            IModalNavigationControl modalNavigationControl,
            IModalNavigationStore modalNavigationStore )
        {
            _modalNavigationControl = modalNavigationControl;
            _modalNavigationStore = modalNavigationStore;
            _paymentStore = paymentStore;
            _logger = logger;
            _paymentService = paymentService;
            _memberService = memberService;

            _payments.CollectionChanged += PaymentsOnCollectionChanged;
            _paymentStore.OnPaymentAdded += PaymentStoreOnOnPaymentsChanged;

            // Initialize commands
            OpenUpdateDialogCommand = new MvxCommand<int>(ExecuteOpenUpdateDialog);
            OpenDeleteDialogCommand = new MvxCommand<int>(ExecuteOpenDeleteDialog);
            OpenAddDialogCommand = new MvxCommand(ExecuteOpenAddDialog);
            CloseCommand = new MvxCommand(ExecuteClose);
        }

        #region Event Handlers

        /// <summary>
        ///     Handles the event when a new payment is added to the store.
        /// </summary>
        /// <param name="payment" >The payment that was added.</param>
        private void PaymentStoreOnOnPaymentsChanged( PaymentWithName payment ) => _payments.Add(payment);

        /// <summary>
        ///     Handles changes to the payments collection and raises property changed notifications.
        /// </summary>
        /// <param name="sender" >The source of the event.</param>
        /// <param name="e" >Event data describing the change.</param>
        private void PaymentsOnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
        {
            RaisePropertyChanged(() => Payments);
            RaisePropertyChanged(() => PaymentCount);
        }

        #endregion

        #region Properties

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

        /// <summary>
        ///     Gets the full name of the member.
        /// </summary>
        public string FullName { get; private set; } = string.Empty;

        #endregion

        #region LifeCycle

        /// <summary>
        ///     Prepares the ViewModel with the specified member ID parameter.
        /// </summary>
        /// <param name="parameter" >The member ID.</param>
        public override void Prepare( int parameter )
        {
            _memberId = parameter;
            _logger.LogInformation("Preparing PaymentDetailsFormViewModel with parameter: {Parameter}", parameter);

            Member? member = _memberService.GetMemberById(_memberId);
            if (member == null)
            {
                _logger.LogWarning("Member with ID {MemberId} not found.", _memberId);
                return;
            }
            FullName = $"{member.FirstName} {member.LastName}";
        }

        /// <summary>
        ///     Initializes the ViewModel, loading payments for the member.
        /// </summary>
        public override async Task Initialize()
        {
            _cancellationTokenSource?.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();

            await LoadPaymentsForMemberAsync(_cancellationTokenSource.Token);
            await base.Initialize();
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Command to open the dialog for updating an existing payment.
        /// </summary>
        public IMvxCommand<int> OpenUpdateDialogCommand { get; }

        /// <summary>
        ///     Command to open the dialog for deleting a payment.
        /// </summary>
        public IMvxCommand<int> OpenDeleteDialogCommand { get; }

        /// <summary>
        ///     Command to open the dialog for adding a new payment.
        /// </summary>
        public IMvxCommand OpenAddDialogCommand { get; }

        /// <summary>
        ///     Command for closing the dialog.
        /// </summary>
        public IMvxCommand CloseCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Loads the payments for the current member asynchronously.
        /// </summary>
        /// <param name="cancellationToken" >Token to monitor for cancellation requests.</param>
        private async Task LoadPaymentsForMemberAsync( CancellationToken cancellationToken )
        {
            try
            {
                _payments.Clear();
                _payments = new MvxObservableCollection<PaymentWithName>(_paymentService.GetPaymentsByMemberId(_memberId, cancellationToken));

                await RaisePropertyChanged(() => Payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payments for member with ID {MemberId}", _memberId);
            }
        }

        /// <summary>
        ///     Executes the command to open the update dialog for a payment.
        /// </summary>
        /// <param name="id" >The ID of the payment to update.</param>
        private void ExecuteOpenUpdateDialog( int id ) => _modalNavigationControl.PopUp<PaymentUpdateFormViewModel>(id);

        /// <summary>
        ///     Executes the command to open the delete dialog for a payment.
        /// </summary>
        /// <param name="id" >The ID of the payment to delete.</param>
        private void ExecuteOpenDeleteDialog( int id ) => _modalNavigationControl.PopUp<PaymentDeleteFormViewModel>(id);

        /// <summary>
        ///     Executes the command to open the add-dialog for a payment.
        /// </summary>
        private void ExecuteOpenAddDialog()
        {
            NavigationParameter parameter = new()
            {
                Id = _memberId,
                CallingViewModel = GetType()
            };
            _modalNavigationControl.PopUp<PaymentCreateFormViewModel>(parameter);
        }

        /// <summary>
        ///     Executes the command to close the modal dialog.
        /// </summary>
        private void ExecuteClose()
        {
            _modalNavigationStore.Close();
        }

        #endregion
    }
}
