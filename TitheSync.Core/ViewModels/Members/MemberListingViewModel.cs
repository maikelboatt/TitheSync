using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using TitheSync.Core.Controls;
using TitheSync.Core.Stores;
using TitheSync.Domain.Models;
using TitheSync.Domain.Services;

namespace TitheSync.Core.ViewModels.Members
{
    /// <summary>
    ///     ViewModel for managing and displaying a list of members.
    /// </summary>
    public class MemberListingViewModel:MvxViewModel, IMemberListingViewModel
    {
        /// <summary>
        ///     A cancellation token source to manage cancellation of asynchronous operations
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        ///     Logger instance for logging messages and errors
        /// </summary>
        private readonly ILogger<MemberListingViewModel> _logger;

        /// <summary>
        ///     Store for managing member data
        /// </summary>
        private readonly IMemberStore _memberStore;

        private readonly IMessageService _messageService;

        /// <summary>
        ///     Control for managing modal navigation, such as opening and closing dialogs.
        /// </summary>
        private readonly IModalNavigationControl _modalNavigationControl;

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
        /// <param name="memberStore" >The member store to retrieve members from.</param>
        /// <param name="logger" >The logger instance for logging errors and information.</param>
        /// <param name="modalNavigationControl" >The control to popup modals</param>
        /// <param name="messageService" >The service for displaying messages to the user.</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when <paramref name="memberStore" /> or <paramref name="logger" /> is
        ///     null.
        /// </exception>
        /// .
        public MemberListingViewModel( IMemberStore memberStore, ILogger<MemberListingViewModel> logger, IModalNavigationControl modalNavigationControl, IMessageService messageService )
        {
            try
            {
                _memberStore = memberStore ?? throw new ArgumentNullException(nameof(memberStore));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _modalNavigationControl = modalNavigationControl ?? throw new ArgumentNullException(nameof(modalNavigationControl));
                _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

                _members = new MvxObservableCollection<Member>(_memberStore.Members);

                _members.CollectionChanged += MembersOnCollectionChanged;

                _memberStore.OnMemberAdded += MemberStoreOnOnMemberAdded;


                // Initialize commands
                OpenAddDialogAsyncCommand = new MvxCommand(ExecuteOpenAddDialog);
                OpenUpdateDialogAsyncCommand = new MvxCommand<int>(ExecuteOpenUpdateDialog);
                OpenDeleteDialogAsyncCommand = new MvxCommand<int>(ExecuteOpenDeleteDialog);
                OpenDetailsDialogAsyncCommand = new MvxCommand<int>(ExecuteOpenDetailsDialog);
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
            await base.Initialize();
            await LoadMembersAsync();
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
        ///     Adds the new member to the local members collection.
        /// </summary>
        /// <param name="member" >The member that was added.</param>
        private void MemberStoreOnOnMemberAdded( Member member )
        {
            _members.Add(member);
        }

        #endregion

        #region Properties

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
        public List<Member> SelectedMembers => [..Members.Where(m => m.IsSelected)];

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
        public IMvxCommand<int> OpenDetailsDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for making batch payments
        /// </summary>
        public IMvxCommand<List<Member>> OpenBatchPaymentDialogAsyncCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Asynchronously loads the members from the member store.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadMembersAsync()
        {
            IsLoading = true;
            try
            {
                // Simulate loading members
                // await Task.Delay(3000, _cancellationTokenSource.Token); // Example delay

                _members.Clear();
                await _memberStore.LoadMemberAsync(_cancellationTokenSource.Token);

                // Add loaded members to the collection
                foreach (Member member in _memberStore.Members)
                {
                    _members.Add(member);
                }

                UpdateView();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("LoadMembersAsync operation was canceled.");
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
        private void ExecuteOpenDeleteDialog( int id ) => throw new NotImplementedException();

        /// <summary>
        ///     Executes the logic to open the details dialog for a member.
        /// </summary>
        /// <param name="id" >The ID of the member.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private void ExecuteOpenDetailsDialog( int id ) => throw new NotImplementedException();

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
            // _modalNavigationControl.PopUp<BatchPaymentFormViewModel>(selected);
        }

        #endregion
    }
}
