using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections.Specialized;
using TitheSync.Core.Controls;
using TitheSync.Core.Stores;
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

        /// <summary>
        ///     Control for managing modal navigation, such as opening and closing dialogs.
        /// </summary>
        private readonly IModalNavigationControl _modalNavigationControl;

        private bool _isLoading;

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberListingViewModel" /> class.
        /// </summary>
        /// <param name="memberStore" >The member store to retrieve members from.</param>
        /// <param name="logger" >The logger instance for logging errors and information.</param>
        /// <param name="modalNavigationControl" >The control to popup modals</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when <paramref name="memberStore" /> or <paramref name="logger" /> is
        ///     null.
        /// </exception>
        /// .
        public MemberListingViewModel( IMemberStore memberStore, ILogger<MemberListingViewModel> logger, IModalNavigationControl modalNavigationControl )
        {
            try
            {
                _memberStore = memberStore ?? throw new ArgumentNullException(nameof(memberStore));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _modalNavigationControl = modalNavigationControl ?? throw new ArgumentNullException(nameof(modalNavigationControl));

                _members.CollectionChanged += MembersOnCollectionChanged;


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
        public IEnumerable<Member> Members { get; } =
        [
            new(1, "John", "Doe", "1234567890", "Male", true, "123 Main St", OrganizationEnum.MensFellowship, BibleClassEnum.ProfDanso),
            new(2, "Jane", "Smith", "9876543210", "Female", false, "456 Elm St", OrganizationEnum.WomensFellowship, BibleClassEnum.MrLartey),
            new(3, "Alice", "Johnson", "5555555555", "Female", true, "789 Oak St", OrganizationEnum.Choir, BibleClassEnum.EmeliaAkrofi),
            new(4, "Bob", "Brown", "4444444444", "Male", false, "321 Pine St", OrganizationEnum.Suwma, BibleClassEnum.AbrahamDadzie),
            new(5, "Charlie", "Davis", "3333333333", "Male", true, "654 Maple St", OrganizationEnum.YouthFellowship, BibleClassEnum.AtoPrempeh),
            new(6, "Emily", "Wilson", "2222222222", "Female", false, "987 Birch St", OrganizationEnum.SingingBand, BibleClassEnum.MichaelKumi),
            new(7, "Frank", "Taylor", "1111111111", "Male", true, "159 Cedar St", OrganizationEnum.GirlsFellowship, BibleClassEnum.JacobBiney),
            new(8, "Grace", "Anderson", "6666666666", "Female", false, "753 Walnut St", OrganizationEnum.BoysAndGirlsBrigade, BibleClassEnum.AuntyAggie),
            new(9, "Henry", "Thomas", "7777777777", "Male", true, "852 Spruce St", OrganizationEnum.MensFellowship, BibleClassEnum.ProfDanso),
            new(10, "Ivy", "Moore", "8888888888", "Female", false, "951 Ash St", OrganizationEnum.WomensFellowship, BibleClassEnum.MrLartey)
        ];

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        ///     Gets the count of members in the collection.
        /// </summary>
        public int MemberCount => Members?.Count() ?? 0;

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
                await Task.Delay(3000, _cancellationTokenSource.Token); // Example delay

                // await _memberStore.LoadMemberAsync(_cancellationTokenSource.Token);
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
        private void ExecuteOpenUpdateDialog( int id )
        {
            List<Member> selected = SelectedMembers;
        }

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
        private void ExecuteOpenBatchPaymentDialog( List<Member>? selectedMembers ) => throw new NotImplementedException();

        #endregion
    }
}
