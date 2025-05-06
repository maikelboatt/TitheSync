using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.Specialized;
using TitheSync.Core.Stores;
using TitheSync.Domain.Models;

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

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberListingViewModel" /> class.
        /// </summary>
        /// <param name="memberStore" >The member store to retrieve members from.</param>
        /// <param name="logger" >The logger instance for logging errors and information.</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when <paramref name="memberStore" /> or <paramref name="logger" /> is
        ///     null.
        /// </exception>
        public MemberListingViewModel( IMemberStore memberStore, ILogger<MemberListingViewModel> logger )
        {
            try
            {
                _memberStore = memberStore ?? throw new ArgumentNullException(nameof(memberStore));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                _members.CollectionChanged += MembersOnCollectionChanged;


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

        #endregion

        /// <summary>
        ///     Backing field for the list of members, initialized from the member store.
        /// </summary>
        private MvxObservableCollection<Member> _members => new(_memberStore.Members.ToList());

        /// <summary>
        ///     Initializes the ViewModel asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task Initialize()
        {
            await base.Initialize();
            // await CreateMemberAsync();
            await LoadMembersAsync();
        }

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

        #region Properties

        /// <summary>
        ///     Gets the collection of members.
        /// </summary>
        public IEnumerable<Member> Members => _members;

        /// <summary>
        ///     Gets the count of members in the collection.
        /// </summary>
        public int MemberCount => Members?.Count() ?? 0;

        #endregion

        #region Commands

        /// <summary>
        ///     Command to open the dialog for adding a new member.
        /// </summary>
        public IMvxAsyncCommand OpenAddDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for updating an existing member.
        /// </summary>
        public IMvxAsyncCommand<int> OpenUpdateDialogAsyncCommand { get; }

        /// <summary>
        ///     Command to open the dialog for deleting an existing member.
        /// </summary>
        public IMvxAsyncCommand<int> OpenDeleteDialogAsyncCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Asynchronously loads the members from the member store.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadMembersAsync()
        {
            try
            {
                await _memberStore.LoadMemberAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("LoadMembersAsync operation was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading members.");
            }
        }

        /// <summary>
        ///     Cancels the ongoing loading operation for members.
        /// </summary>
        private void CancelLoading() => _cancellationTokenSource.Cancel();

        /// <summary>
        ///     Executes the logic to open the dialog for adding a new member.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task ExecuteOpenAddDialog() => throw
            // _modalNavigationControl.PopUp<RemovalEventCreateFormViewModel>(AnimalId); // Pass AnimalId to Create form
            new NotImplementedException();

        /// <summary>
        ///     Executes the logic to open the dialog for updating an existing member.
        /// </summary>
        /// <param name="id" >The ID of the member to update.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task ExecuteOpenUpdateDialog( int id, CancellationToken cancellationToken = default ) => throw new NotImplementedException();

        /// <summary>
        ///     Executes the logic to open the dialog for deleting an existing member.
        /// </summary>
        /// <param name="id" >The ID of the member to delete.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task ExecuteOpenDeleteDialog( int id, CancellationToken cancellationToken = default ) => throw new NotImplementedException();

        #endregion
    }
}
