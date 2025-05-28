using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Windows;
using TitheSync.Core.Stores;
using TitheSync.Domain.Models;
using TitheSync.Infrastructure.Services;
using TitheSync.Service.Services;

namespace TitheSync.Core.ViewModels.Payments
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PaymentCreateFormViewModel" /> class.
    /// </summary>
    /// <param name="modalNavigationStore" >The modal navigation store for managing navigation.</param>
    /// <param name="paymentStore" >The payment store for managing payment data.</param>
    /// <param name="logger" >The logger for logging information.</param>
    /// <param name="dateConverterService" >The service for converting date objects.</param>
    public class PaymentDeleteFormViewModel(
        IModalNavigationStore modalNavigationStore,
        IPaymentStore paymentStore,
        ILogger<PaymentCreateFormViewModel> logger,
        IDateConverterService dateConverterService,
        IMessageService messageService ):MvxViewModel<int>, IPaymentDeleteFormViewModel
    {
        private decimal _amount;
        private DateTime _datePaid;
        private int _paymentId;

        #region Properties

        public DateTime DatePaid
        {
            get => _datePaid;
            set => SetProperty(ref _datePaid, value);
        }
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
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
        ///     Initializes the ViewModel asynchronously. Retrieves the payment's payment information using the stored payment ID.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        public override async Task Initialize()
        {
            await base.Initialize();
            PaymentWithName? payment = paymentStore.PaymentWithNames.FirstOrDefault(p => p.PaymentId == _paymentId);
            if (payment == null)
            {
                logger.LogWarning("Payment with ID {PaymentId} not found", _paymentId);
                return;
            }
            PopulateFormFields(payment);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Gets the command for submitting the record.
        /// </summary>
        public IMvxAsyncCommand DeleteRecordCommand => new MvxAsyncCommand(ExecuteDeleteRecord);

        /// <summary>
        ///     Gets the command for canceling the record update.
        /// </summary>
        public IMvxCommand CancelRecordCommand => new MvxCommand(ExecuteCancelRecord);

        #endregion

        #region Methods

        /// <summary>
        ///     Cancels the record update and closes the modal.
        /// </summary>
        private void ExecuteCancelRecord() => modalNavigationStore.Close();


        /// <summary>
        ///     Prompts the user for confirmation and deletes the record if confirmed.
        /// </summary>
        /// <param name="cancellationToken" >The cancellation token for the operation.</param>
        private async Task ExecuteDeleteRecord( CancellationToken cancellationToken )
        {

            if (messageService.Show(
                    "Are you sure you want to delete the record?",
                    "Confirmation",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) is not MessageBoxResult.OK)
            {
                return;
            }
            await OnDeleteConfirm(cancellationToken);
        }

        /// <summary>
        ///     Deletes the member record and closes the modal navigation store.
        /// </summary>
        /// <param name="cancellationToken" ></param>
        private async Task OnDeleteConfirm( CancellationToken cancellationToken )
        {
            await paymentStore.DeletePaymentAsync(_paymentId, cancellationToken);
            modalNavigationStore.Close();
        }

        private void PopulateFormFields( PaymentWithName payment )
        {

            _amount = payment.Amount;
            _datePaid = dateConverterService.ConvertToDateTime(payment.DatePaid);
        }

        #endregion
    }
}
