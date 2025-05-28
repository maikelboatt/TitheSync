using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.Specialized;
using TitheSync.Core.Controls;
using TitheSync.Core.Parameters;
using TitheSync.Core.Stores;
using TitheSync.Domain.Models;

namespace TitheSync.Core.ViewModels.Payments
{
    public class PaymentDetailsFormViewModel:MvxViewModel<int>, IPaymentDetailsViewModel
    {
        private readonly ILogger<PaymentDetailsFormViewModel> _logger;
        private readonly IMemberStore _memberStore;
        private readonly IModalNavigationControl _modalNavigationControl;
        private readonly IModalNavigationStore _modalNavigationStore;
        private readonly IPaymentStore _paymentStore;

        /// <summary>
        ///     A cancellation token source to manage cancellation of asynchronous operations.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        private int _memberId;
        private MvxObservableCollection<PaymentWithName> _payments = [];

        public PaymentDetailsFormViewModel( ILogger<PaymentDetailsFormViewModel> logger, IPaymentStore paymentStore, IMemberStore memberStore,
            IModalNavigationControl modalNavigationControl,
            IModalNavigationStore modalNavigationStore )
        {
            _modalNavigationControl = modalNavigationControl;
            _modalNavigationStore = modalNavigationStore;
            _paymentStore = paymentStore;
            _memberStore = memberStore;
            _logger = logger;

            _payments.CollectionChanged += PaymentsOnCollectionChanged;
            _paymentStore.OnPaymentAdded += PaymentStoreOnOnPaymentsChanged;

            // Initialize commands
            OpenUpdateDialogCommand = new MvxCommand<int>(ExecuteOpenUpdateDialog);
            OpenDeleteDialogCommand = new MvxCommand<int>(ExecuteOpenDeleteDialog);
            OpenAddDialogCommand = new MvxCommand(ExecuteOpenAddDialog);
            CloseCommand = new MvxCommand(ExecuteClose);
        }

        #region Event Handlers

        private void PaymentStoreOnOnPaymentsChanged( PaymentWithName payment ) => _payments.Add(payment);

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

        public string FullName { get; private set; } = string.Empty;

        #endregion


        #region LifeCycle

        public override void Prepare( int parameter )
        {
            _memberId = parameter;
            _logger.LogInformation("Preparing PaymentDetailsFormViewModel with parameter: {Parameter}", parameter);

            Member? member = _memberStore.Members.FirstOrDefault(m => m.MemberId == _memberId);
            if (member == null)
            {
                _logger.LogWarning("Member with ID {MemberId} not found.", _memberId);
                return;
            }
            FullName = $"{member.FirstName} {member.LastName}";
        }

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

        private async Task LoadPaymentsForMemberAsync( CancellationToken cancellationToken )
        {
            try
            {
                _payments.Clear();
                _payments = new MvxObservableCollection<PaymentWithName>(_paymentStore.GetPaymentsByMemberId(_memberId));

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
        /// <param name="id" ></param>
        private void ExecuteOpenUpdateDialog( int id ) => _modalNavigationControl.PopUp<PaymentUpdateFormViewModel>(id);

        /// <summary>
        ///     Executes the command to open the delete dialog for a payment.|
        /// </summary>
        /// <param name="id" ></param>
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
        ///     Executes the command to close the modal.
        /// </summary>
        private void ExecuteClose()
        {
            _modalNavigationStore.Close();
        }

        #endregion
    }
}
