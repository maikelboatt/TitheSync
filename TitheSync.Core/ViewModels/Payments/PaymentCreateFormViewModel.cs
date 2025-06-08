using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections;
using System.ComponentModel;
using TitheSync.ApplicationState.Stores;
using TitheSync.ApplicationState.Stores.Members;
using TitheSync.Business.Services.Members;
using TitheSync.Business.Services.Payments;
using TitheSync.Core.Parameters;
using TitheSync.Core.Validation;
using TitheSync.Domain.Models;
using TitheSync.Infrastructure.Models;
using TitheSync.Infrastructure.Services;

namespace TitheSync.Core.ViewModels.Payments
{
    public class PaymentCreateFormViewModel:MvxViewModel<NavigationParameter>, IPaymentCreateFormViewModel, INotifyDataErrorInfo
    {
        private readonly IDateConverterService _dateConverterService;
        private readonly ILogger<PaymentCreateFormViewModel> _logger;
        private readonly IMemberService _memberService;
        private readonly IMemberStore _memberStore;
        private readonly IModalNavigationStore _modalNavigationStore;
        private readonly INotificationStore _notificationStore;
        private readonly IPaymentService _paymentService;
        private readonly PaymentRecordValidation _validator = new();
        private decimal _amount;
        private DateTime _datePaid;
        private int _memberId;


        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentCreateFormViewModel" /> class.
        /// </summary>
        /// <param name="modalNavigationStore" >The modal navigation store for managing navigation.</param>
        /// <param name="paymentService" >The payment service for handling the creation of payments.</param>
        /// <param name="memberStore" >The member store for managing member data.</param>
        /// <param name="memberService" >The member service for handling the creation of members</param>
        /// <param name="logger" >The logger for logging information.</param>
        /// <param name="dateConverterService" >The service for converting date objects.</param>
        /// <param name="notificationStore" >The notification store to manage notifications.</param>
        public PaymentCreateFormViewModel( IModalNavigationStore modalNavigationStore, IPaymentService paymentService, IMemberStore memberStore, IMemberService memberService,
            ILogger<PaymentCreateFormViewModel> logger,
            IDateConverterService dateConverterService, INotificationStore notificationStore )
        {
            _modalNavigationStore = modalNavigationStore;
            _paymentService = paymentService;
            _memberStore = memberStore;
            _memberService = memberService;
            _logger = logger;
            _dateConverterService = dateConverterService;
            _notificationStore = notificationStore;

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

        public string FullName { get; private set; } = string.Empty;

        #endregion

        #region LifeCycle

        /// <summary>
        ///     Cleans up resources when the view is destroyed.
        /// </summary>
        /// <param name="viewFinishing" >Indicates whether the view is finishing.</param>
        public override void ViewDestroy( bool viewFinishing = true )
        {
            _validator.ErrorsChanged -= ValidatorOnErrorsChanged;
            base.ViewDestroy(viewFinishing);
        }

        /// <summary>
        ///     Prepares the ViewModel with the given navigation parameter.
        ///     Sets the member ID or retrieves payment information based on the calling ViewModel.
        ///     Logs the preparation process for debugging and traceability.
        /// </summary>
        /// <param name="parameter" >
        ///     The <see cref="NavigationParameter" /> containing information about the calling ViewModel and relevant IDs.
        /// </param>
        public override void Prepare( NavigationParameter parameter )
        {
            _memberId = parameter.Id;
            _logger.LogInformation("Preparing PaymentCreateFormViewModel from {CallingViewModel} with member ID: {MemberId}", parameter.CallingViewModel.Name, _memberId);
        }


        /// <summary>
        ///     Initializes the ViewModel asynchronously. Retrieves the member's payment information
        ///     using the stored member ID and sets the <see cref="FullName" /> property if found.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        public override Task Initialize()
        {
            Member? member = _memberStore.Members.FirstOrDefault(m => m.MemberId == _memberId);
            if (member is null) return base.Initialize();

            FullName = $"{member.FirstName} {member.LastName}";
            return base.Initialize();
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Gets the command for submitting the record.
        /// </summary>
        public IMvxAsyncCommand SubmitRecordCommand
        {
            get;
        }

        /// <summary>
        ///     Gets the command for canceling the record update.
        /// </summary>
        public IMvxCommand CancelRecordCommand
        {
            get;
        }

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
            await _paymentService.AddPaymentAsync(payment, cancellationToken);

            _notificationStore.AddNotification(
                new Notification
                {
                    Title = "Payment Added",
                    Message = $"Payment of {payment.Amount:C} for {FullName} has been added successfully.",
                    TimeStamp = DateTime.UtcNow,
                    Type = "Success"
                }
            );

            _modalNavigationStore.Close();
        }

        private PaymentWithName GetPaymentFromFields()
        {
            Member? result = _memberService.GetMemberById(_memberId);
            return result is not null
                ? new PaymentWithName(1, _memberId, Amount, _dateConverterService.ConvertToDateOnly(DatePaid), result.FirstName, result.LastName)
                : new PaymentWithName(1, _memberId, Amount, _dateConverterService.ConvertToDateOnly(DatePaid), string.Empty, string.Empty);
        }

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
