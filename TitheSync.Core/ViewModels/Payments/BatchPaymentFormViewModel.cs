using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using TitheSync.ApplicationState.Stores;
using TitheSync.Business.Services.Members;
using TitheSync.Business.Services.Payments;
using TitheSync.Domain.Models;
using TitheSync.Infrastructure.Models;
using TitheSync.Infrastructure.Services;

namespace TitheSync.Core.ViewModels.Payments
{
    /// <summary>
    ///     ViewModel for handling batch payments for multiple members.
    /// </summary>
    public class BatchPaymentFormViewModel:MvxViewModel<List<Member>>, IBatchPaymentFormViewModel
    {
        private readonly IDateConverterService _dateConverterService;
        private readonly IMemberService _memberService;
        private readonly IModalNavigationStore _modalNavigationStore;
        private readonly INotificationStore _notificationStore;
        private readonly IPaymentService _paymentService;
        private DateTime _paymentDate = DateTime.Today;
        private IEnumerable<Member> _selectedMembers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BatchPaymentFormViewModel" /> class.
        /// </summary>
        /// <param name="paymentService" >Service for handling payments.</param>
        /// <param name="memberService" >Service for handling members.</param>
        /// <param name="dateConverterService" >Service for date conversion.</param>
        /// <param name="notificationStore" >Store for notifications.</param>
        /// <param name="modalNavigationStore" >Store for modal navigation.</param>
        public BatchPaymentFormViewModel( IPaymentService paymentService, IMemberService memberService, IDateConverterService dateConverterService, INotificationStore notificationStore,
            IModalNavigationStore modalNavigationStore )
        {
            _paymentService = paymentService;
            _memberService = memberService;
            _dateConverterService = dateConverterService;
            _notificationStore = notificationStore;
            _modalNavigationStore = modalNavigationStore;
            SubmitCommand = new MvxAsyncCommand(ExecuteSubmitBatchPayment, CanSubmit);
            CloseCommand = new MvxCommand(ExecuteCloseDialog);

            Entries.CollectionChanged += ( s, e ) => SubmitCommand.RaiseCanExecuteChanged();
        }

        #region LifeCycle Methods

        /// <summary>
        ///     Prepares the ViewModel with the selected members.
        /// </summary>
        /// <param name="parameter" >The list of selected members.</param>
        public override void Prepare( List<Member> parameter )
        {
            _selectedMembers = parameter;
        }

        /// <summary>
        ///     Populates the Entries collection when the dialog opens.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task Initialize()
        {
            Entries.Clear();
            foreach (Member m in _selectedMembers)
            {
                Entries.Add(
                    new BatchPaymentEntryViewModel
                    {
                        MemberId = m.MemberId,
                        FullName = $"{m.FirstName} {m.LastName}"
                    });
            }
            return base.Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the collection of batch payment entries.
        /// </summary>
        public ObservableCollection<BatchPaymentEntryViewModel> Entries { get; } = [];

        /// <summary>
        ///     Gets or sets the payment date for the batch.
        /// </summary>
        public DateTime PaymentDate
        {
            get => _paymentDate;
            set
            {
                if (SetProperty(ref _paymentDate, value))
                    SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Gets the command to submit the batch payment.
        /// </summary>
        public IMvxAsyncCommand SubmitCommand { get; }

        /// <summary>
        ///     Gets the command to close the dialog.
        /// </summary>
        public IMvxCommand CloseCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Closes the batch payment dialog.
        /// </summary>
        private void ExecuteCloseDialog() => _modalNavigationStore.Close();

        /// <summary>
        ///     Executes the batch payment submission.
        /// </summary>
        /// <param name="cancellationToken" >Cancellation token.</param>
        private async Task ExecuteSubmitBatchPayment( CancellationToken cancellationToken )
        {
            foreach (BatchPaymentEntryViewModel entry in Entries)
            {
                Member? member = _memberService.GetMemberById(entry.MemberId);
                PaymentWithName payment = member is not null
                    ? new PaymentWithName(1, entry.MemberId, entry.Amount, _dateConverterService.ConvertToDateOnly(PaymentDate), member.FirstName, member.LastName)
                    : new PaymentWithName(1, entry.MemberId, entry.Amount, _dateConverterService.ConvertToDateOnly(PaymentDate), string.Empty, string.Empty);

                await _paymentService.AddPaymentAsync(payment, cancellationToken);
            }

            AddNotifications();

            _modalNavigationStore.Close();
        }

        /// <summary>
        ///     Adds a success notification indicating that batch payments have been added.
        /// </summary>
        private void AddNotifications()
        {

            _notificationStore.AddNotification(
                new Notification
                {
                    Title = "Batch Payment Added",
                    Message = $"Payments for {Entries.Count} members have been added successfully.",
                    TimeStamp = DateTime.UtcNow,
                    Type = "Success"
                }
            );
        }


        /// <summary>
        ///     Determines whether the batch payment can be submitted.
        /// </summary>
        /// <returns>True if the batch payment can be submitted; otherwise, false.</returns>
        private bool CanSubmit()
        {
            return
                Entries.Count > 0
                && Entries.All(e => e.Amount > 0);
        }

        #endregion
    }
}
