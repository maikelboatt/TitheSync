using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using TitheSync.ApplicationState.Stores.Members;
using TitheSync.Business.Services;
using TitheSync.Business.Services.Members;
using TitheSync.Core.Controls;
using TitheSync.Core.Parameters;
using TitheSync.Core.ViewModels.Payments;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;

namespace TitheSync.Core.ViewModels.Members
{
    /// <summary>
    ///     ViewModel for managing and displaying a list of members.
    /// </summary>
    public class MemberListingViewModel:MvxViewModel, IMemberListingViewModel
    {
        /// <summary>
        ///     Logger instance for logging messages and errors
        /// </summary>
        private readonly ILogger<MemberListingViewModel> _logger;

        private readonly IMemberService _memberService;

        /// <summary>
        ///     Store for managing member data
        /// </summary>
        private readonly IMemberStore _memberStore;

        /// <summary>
        ///     Service for displaying message boxes
        /// </summary>
        private readonly IMessageService _messageService;

        /// <summary>
        ///     Control for managing modal navigation, such as opening and closing dialogs.
        /// </summary>
        private readonly IModalNavigationControl _modalNavigationControl;

        /// <summary>
        ///     A cancellation token source to manage cancellation of asynchronous operations
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        ///     Indicates whether the ViewModel is currently performing a loading operation.
        /// </summary>
        private bool _isLoading;

        /// <summary>
        ///     Backing field for the list of members, initialized from the member store.
        /// </summary>
        private MvxObservableCollection<Member> _members;


        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberListingViewModel" /> class.
        /// </summary>
        /// <param name="memberService" >The service to load all members from the repository</param>
        /// <param name="memberStore" >The member store to retrieve members from.</param>
        /// <param name="logger" >The logger instance for logging errors and information.</param>
        /// <param name="modalNavigationControl" >The control to popup modals</param>
        /// <param name="messageService" >The service for displaying messages to the user.</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when <paramref name="memberStore" /> or <paramref name="logger" /> is
        ///     null.
        /// </exception>
        /// .
        public MemberListingViewModel( IMemberService memberService, IMemberStore memberStore, ILogger<MemberListingViewModel> logger, IModalNavigationControl modalNavigationControl,
            IMessageService messageService )
        {
            try
            {
                _memberStore = memberStore ?? throw new ArgumentNullException(nameof(memberStore));
                _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _modalNavigationControl = modalNavigationControl ?? throw new ArgumentNullException(nameof(modalNavigationControl));
                _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

                _members = new MvxObservableCollection<Member>(_memberStore.Members);

                _members.CollectionChanged += MembersOnCollectionChanged;

                _memberStore.OnMemberAdded += MemberStoreOnOnMemberAdded;
                _memberStore.OnMemberDeleted += MemberStoreOnOnMemberDeleted;
                _memberStore.OnMemberUpdated += MemberStoreOnOnMemberUpdated;


                // Initialize commands
                OpenAddDialogAsyncCommand = new MvxCommand(ExecuteOpenAddDialog);
                OpenUpdateDialogAsyncCommand = new MvxCommand<int>(ExecuteOpenUpdateDialog);
                OpenDeleteDialogAsyncCommand = new MvxCommand<int>(ExecuteOpenDeleteDialog);
                OpenAddMemberPaymentDialogAsyncCommand = new MvxCommand<int>(ExecuteOpenAddMemberPaymentDialog);
                OpenBatchPaymentDialogAsyncCommand = new MvxCommand<List<Member>>(ExecuteOpenBatchPaymentDialog);

            }
            catch (ArgumentNullException e)
            {
                _logger?.LogError(e, "ArgumentNullException: {Message}", e.Message);
                throw;
            }
        }

        #endregion

        #region Lifecycle

        /// <summary>
        ///     Initializes the ViewModel asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task Initialize()
        {
            _cancellationTokenSource?.CancelAsync();
            _cancellationTokenSource = new CancellationTokenSource();
            await LoadMembersAsync(_cancellationTokenSource.Token);
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
            _members.CollectionChanged -= MembersOnCollectionChanged;
            _memberStore.OnMemberAdded -= MemberStoreOnOnMemberAdded;
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
        ///     Event handler for when the members collection changes.
        ///     Raises a property changed notification for the Members property.
        /// </summary>
        /// <param name="sender" >The source of the event.</param>
        /// <param name="e" >The event data.</param>
        private void MembersOnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
        {
            RaisePropertyChanged(() => Members);
        }

        /// <summary>
        ///     Event handler for when a new member is added to the member store.
        ///     Adds the new member to the local-members collection.
        /// </summary>
        /// <param name="member" >The member that was added.</param>
        private void MemberStoreOnOnMemberAdded( Member member ) => _members.Add(member);

        /// <summary>
        ///     Handles the event when a member is updated in the member store.
        ///     Updates the corresponding member in the local collection if it exists,
        ///     and raises a property changed notification for the Members property.
        /// </summary>
        /// <param name="member" >The updated member object.</param>
        private void MemberStoreOnOnMemberUpdated( Member member )
        {
            // Update the member in the local collection if it exists
            // I could use IndexOf
            int index = _members.ToList().FindIndex(m => m.MemberId == member.MemberId);
            if (index < 0) return;
            _members[index] = member;
            RaisePropertyChanged(() => Members);
        }

        /// <summary>
        ///     Event handler for when a member is deleted from the member store.
        ///     Removes the member from the local-members collection.
        /// </summary>
        /// <param name="member" >The member that was removed.</param>
        private void MemberStoreOnOnMemberDeleted( Member member ) => _members.Remove(member);

        #endregion

        #region Properties

        public IEnumerable<Member> DummyMembers =>
        [
            new(1, "John", "Doe", "1234567890", "Male", true, "123 Main St", OrganizationEnum.MensFellowship, BibleClassEnum.ProfDanso),
            new(2, "Jane", "Smith", "0987654321", "Female", false, "456 Elm St", OrganizationEnum.WomensFellowship, BibleClassEnum.MrLartey),
            new(3, "Michael", "Brown", "1112223333", "Male", false, "789 Oak St", OrganizationEnum.Choir, BibleClassEnum.EmeliaAkrofi),
            new(4, "Emily", "Davis", "4445556666", "Female", true, "321 Pine St", OrganizationEnum.Suwma, BibleClassEnum.AbrahamDadzie),
            new(5, "Chris", "Wilson", "7778889999", "Male", false, "654 Maple St", OrganizationEnum.YouthFellowship, BibleClassEnum.AtoPrempeh),
            new(6, "Sarah", "Taylor", "1231231234", "Female", false, "987 Cedar St", OrganizationEnum.SingingBand, BibleClassEnum.MichaelKumi),
            new(7, "David", "Anderson", "4564564567", "Male", true, "159 Birch St", OrganizationEnum.GirlsFellowship, BibleClassEnum.JacobBiney),
            new(8, "Laura", "Thomas", "7897897890", "Female", false, "753 Walnut St", OrganizationEnum.BoysAndGirlsBrigade, BibleClassEnum.AuntyAggie),
            new(9, "James", "Moore", "3213213210", "Male", false, "852 Spruce St", OrganizationEnum.MensFellowship, BibleClassEnum.ProfDanso),
            new(10, "Anna", "Jackson", "6546546543", "Female", true, "951 Fir St", OrganizationEnum.WomensFellowship, BibleClassEnum.MrLartey),
            new(11, "Robert", "White", "9879879876", "Male", false, "147 Ash St", OrganizationEnum.Choir, BibleClassEnum.EmeliaAkrofi),
            new(12, "Sophia", "Harris", "2582582589", "Female", false, "369 Willow St", OrganizationEnum.Suwma, BibleClassEnum.AbrahamDadzie),
            new(13, "Daniel", "Martin", "7417417412", "Male", true, "753 Poplar St", OrganizationEnum.YouthFellowship, BibleClassEnum.AtoPrempeh),
            new(14, "Olivia", "Thompson", "8528528523", "Female", false, "159 Redwood St", OrganizationEnum.SingingBand, BibleClassEnum.MichaelKumi),
            new(15, "Matthew", "Garcia", "9639639634", "Male", false, "357 Cypress St", OrganizationEnum.GirlsFellowship, BibleClassEnum.JacobBiney),
            new(16, "Isabella", "Martinez", "1471471475", "Female", true, "951 Palm St", OrganizationEnum.BoysAndGirlsBrigade, BibleClassEnum.AuntyAggie),
            new(17, "Andrew", "Robinson", "3693693696", "Male", false, "753 Magnolia St", OrganizationEnum.MensFellowship, BibleClassEnum.ProfDanso),
            new(18, "Mia", "Clark", "2582582587", "Female", false, "159 Dogwood St", OrganizationEnum.WomensFellowship, BibleClassEnum.MrLartey),
            new(19, "Joshua", "Rodriguez", "7417417418", "Male", true, "357 Cherry St", OrganizationEnum.Choir, BibleClassEnum.EmeliaAkrofi),
            new(20, "Emma", "Lewis", "8528528529", "Female", false, "951 Beech St", OrganizationEnum.Suwma, BibleClassEnum.AbrahamDadzie)
        ];

        /// <summary>
        ///     Gets the collection of members.
        /// </summary>
        public MvxObservableCollection<Member> Members
        {
            get => _members;
            private set
            {
                _members = value;
                RaisePropertyChanged(() => Members);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the ViewModel is currently performing a loading operation.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        ///     Gets the count of members in the collection.
        /// </summary>
        public int MemberCount => Members.Count;

        /// <summary>
        ///     Gets the list of members that are currently selected.
        /// </summary>
        private List<Member> SelectedMembers => [..Members.Where(m => m.IsSelected)];

        #endregion

        #region Commands

        /// <summary>
        ///     Command to open the dialog for adding a new member.
        /// </summary>
        public IMvxCommand OpenAddDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for updating an existing member.
        /// </summary>
        public IMvxCommand<int> OpenUpdateDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for deleting an existing member.
        /// </summary>
        public IMvxCommand<int> OpenDeleteDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for viewing details of a member.
        /// </summary>
        public IMvxCommand<int> OpenAddMemberPaymentDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for making batch payments
        /// </summary>
        public IMvxCommand<List<Member>> OpenBatchPaymentDialogAsyncCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Asynchronously loads the members from the member store.
        /// </summary>
        /// <param name="cancellationToken" >The token to manage cancellation of the LoadMembersAsync</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadMembersAsync( CancellationToken cancellationToken )
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                // Simulate loading members
                // await Task.Delay(3000, _cancellationTokenSource.Token); // Example delay

                _members.Clear();
                await _memberService.GetMembersAsync(cancellationToken);
                // Add loaded members to the collection
                foreach (Member member in _memberStore.Members)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _members.Add(member);
                }

                await RaisePropertyChanged(() => Members);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Loading members was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading members.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateView()
        {
            // Update the view with the loaded members
            RaisePropertyChanged(() => Members);
            RaisePropertyChanged(() => SelectedMembers);
        }

        /// <summary>
        ///     Cancels the ongoing loading operation for members.
        /// </summary>
        private void CancelLoading() => _cancellationTokenSource.Cancel();

        /// <summary>
        ///     Executes the logic to open the dialog for adding a new member.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private void ExecuteOpenAddDialog() => _modalNavigationControl.PopUp<MemberCreateFormViewModel>(1);

        /// <summary>
        ///     Executes the logic to open the dialog for updating an existing member.
        /// </summary>
        /// <param name="id" >The ID of the member to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private void ExecuteOpenUpdateDialog( int id ) => _modalNavigationControl.PopUp<MemberUpdateFormViewModel>(id);

        /// <summary>
        ///     Executes the logic to open the dialog for deleting an existing member.
        /// </summary>
        /// <param name="id" >The ID of the member to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private void ExecuteOpenDeleteDialog( int id ) => _modalNavigationControl.PopUp<MemberDeleteFormViewModel>(id);

        /// <summary>
        ///     Executes the logic to add payments for a member.
        /// </summary>
        /// <param name="id" >The ID of the member.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private void ExecuteOpenAddMemberPaymentDialog( int id )
        {
            NavigationParameter parameter = new()
            {
                Id = id,
                CallingViewModel = GetType()
            };
            _modalNavigationControl.PopUp<PaymentCreateFormViewModel>(parameter);
        }

        /// <summary>
        ///     Executes the logic to open the batch payment dialog.
        /// </summary>
        /// <param name="selectedMembers" >The members making the batch payment</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private void ExecuteOpenBatchPaymentDialog( List<Member>? selectedMembers )
        {
            List<Member> selected = SelectedMembers;
            if (selected.Count == 0)
            {
                _messageService.Show("Please select one or more members for batch payment", "Notification", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            _modalNavigationControl.PopUp<BatchPaymentFormViewModel>(selected);
        }

        #endregion
    }
}
