using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections;
using System.ComponentModel;
using TitheSync.Core.Stores;
using TitheSync.Core.Validation;
using TitheSync.Domain.Models;
using TitheSync.Infrastructure.Services;

namespace TitheSync.Core.ViewModels.Payments
{
    public class PaymentUpdateFormViewModel:MvxViewModel<int>, IPaymentUpdateFormViewModel
    {
        private readonly IDateConverterService _dateConverterService;
        private readonly ILogger<PaymentCreateFormViewModel> _logger;
        private readonly IModalNavigationStore _modalNavigationStore;
        private readonly IPaymentStore _paymentStore;
        private readonly PaymentRecordValidation _validator = new();
        private decimal _amount;
        private DateTime _datePaid;
        private int _memberId;
        private int _paymentId;


        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentCreateFormViewModel" /> class.
        /// </summary>
        /// <param name="modalNavigationStore" >The modal navigation store for managing navigation.</param>
        /// <param name="paymentStore" >The member store for managing member data.</param>
        /// <param name="logger" >The logger for logging information.</param>
        /// <param name="dateConverterService" >The service for converting date objects.</param>
        public PaymentUpdateFormViewModel( IModalNavigationStore modalNavigationStore, IPaymentStore paymentStore, ILogger<PaymentCreateFormViewModel> logger,
            IDateConverterService dateConverterService )
        {
            _modalNavigationStore = modalNavigationStore;
            _paymentStore = paymentStore;
            _logger = logger;
            _dateConverterService = dateConverterService;

            _validator.ErrorsChanged += ValidatorOnErrorsChanged;

            // Initialize commands
            SubmitRecordCommand = new MvxAsyncCommand(ExecuteSubmitRecord, CanSubmit);
            CancelRecordCommand = new MvxCommand(ExecuteCancelRecord);
        }

        /// <summary>
        ///     Handles changes in validation errors.
        /// </summary>
        /// <param name="sender" >The sender of the event.</param>
        /// <param name="e" >The event arguments.</param>
        private void ValidatorOnErrorsChanged( object? sender, DataErrorsChangedEventArgs e )
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(e.PropertyName));
            RaisePropertyChanged(nameof(HasErrors));
            RaisePropertyChanged(nameof(CanSubmit));
        }

        #region Properties

        public DateTime DatePaid
        {
            get => _datePaid;
            set
            {
                if (value.Equals(_datePaid)) return;
                _datePaid = value;
                _validator.Validate(_dateConverterService.ConvertToDateOnly(_datePaid));
                RaisePropertyChanged(() => DatePaid);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (value == _amount) return;
                _amount = value;
                _validator.Validate(_amount);
                RaisePropertyChanged(() => Amount);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region LifeCycle

        /// <summary>
        ///     Prepares the ViewModel with the given parameter.
        /// </summary>
        /// <param name="parameter" >The parameter to prepare the ViewModel with.</param>
        public override void Prepare( int parameter )
        {
            _paymentId = parameter;
        }

        /// <summary>
        ///     Initializes the ViewModel asynchronously. Retrieves the member's payment information using the stored member ID.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        public override async Task Initialize()
        {
            await base.Initialize();
            PaymentWithName? payment = _paymentStore.PaymentWithNames.FirstOrDefault(p => p.PaymentId == _paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment with ID {PaymentId} not found", _paymentId);
                return;
            }
            PopulateFormFields(payment);
            _memberId = payment.PaymentMemberId;
        }

        /// <summary>
        ///     Cleans up resources when the view is destroyed.
        /// </summary>
        /// <param name="viewFinishing" >Indicates whether the view is finishing.</param>
        public override void ViewDestroy( bool viewFinishing = true )
        {
            _validator.ErrorsChanged -= ValidatorOnErrorsChanged;
            base.ViewDestroy(viewFinishing);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Gets the command for submitting the record.
        /// </summary>
        public IMvxAsyncCommand SubmitRecordCommand { get; }

        /// <summary>
        ///     Gets the command for canceling the record update.
        /// </summary>
        public IMvxCommand CancelRecordCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Cancels the record update and closes the modal.
        /// </summary>
        private void ExecuteCancelRecord() => _modalNavigationStore.Close();

        /// <summary>
        ///     Determines whether the submit command can execute.
        /// </summary>
        /// <returns>True if the form is valid; otherwise, false.</returns>
        private bool CanSubmit()
        {
            bool noFieldEmpty = !DatePaid.Equals(default);

            return noFieldEmpty && !HasErrors;
        }


        /// <summary>
        ///     Submits the updated member record asynchronously.
        /// </summary>
        /// <param name="cancellationToken" >The cancellation token for the operation.</param>
        private async Task ExecuteSubmitRecord( CancellationToken cancellationToken )
        {
            if (_validator.HasErrors) return;

            PaymentWithName payment = GetPaymentFromFields();
            await _paymentStore.UpdatePaymentAsync(payment, cancellationToken);

            _modalNavigationStore.Close();
        }

        private void PopulateFormFields( PaymentWithName payment )
        {

            _amount = payment.Amount;
            _datePaid = _dateConverterService.ConvertToDateTime(payment.DatePaid);
        }

        private PaymentWithName GetPaymentFromFields() => new(_paymentId, _memberId, Amount, _dateConverterService.ConvertToDateOnly(DatePaid), string.Empty, string.Empty);

        #endregion

        #region Validation

        /// <summary>
        ///     Gets validation errors for a specific property.
        /// </summary>
        /// <param name="propertyName" >The name of the property to get errors for.</param>
        /// <returns>An enumerable of validation errors.</returns>
        public IEnumerable GetErrors( string? propertyName ) => _validator.GetErrors(propertyName);

        /// <summary>
        ///     Gets a value indicating whether the form has validation errors.
        /// </summary>
        public bool HasErrors => _validator.HasErrors;

        /// <summary>
        ///     Occurs when validation errors change.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        #endregion
    }
}
