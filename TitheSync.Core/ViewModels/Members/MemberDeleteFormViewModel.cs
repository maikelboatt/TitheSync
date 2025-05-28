using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Windows;
using TitheSync.Core.Stores;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;
using TitheSync.Service.Services;

namespace TitheSync.Core.ViewModels.Members
{
    public class MemberDeleteFormViewModel(
        IModalNavigationStore modalNavigationStore,
        IMemberStore memberStore,
        ILogger<MemberCreateFormViewModel> logger,
        IMessageService messageService )
        :MvxViewModel<int>, IMemberDeleteFormViewModel
    {
        private string _address = string.Empty;
        private BibleClassEnum _bibleClass;
        private string _contact = string.Empty;
        private string _firstName = string.Empty;
        private string _gender = string.Empty;
        private bool _isLeader;
        private string _lastName = string.Empty;

        private int _memberId;
        private OrganizationEnum _organization;

        #region LifeCycle

        /// <summary>
        ///     Prepares the ViewModel with the specified member ID.
        /// </summary>
        /// <param name="parameter" >The ID of the member to be prepared.</param>
        public override void Prepare( int parameter )
        {
            logger.LogInformation("Preparing MemberDeleteFormViewModel with parameter: {Parameter}", parameter);
            _memberId = parameter;
        }

        /// <summary>
        ///     Initializes the ViewModel asynchronously.
        /// </summary>
        public override async Task Initialize()
        {
            await base.Initialize();
            Member? member = memberStore.Members.FirstOrDefault(m => m.MemberId == _memberId);
            if (member == null)
            {
                logger.LogError("Member with ID {MemberId} not found", _memberId);
                return;
            }

            PopulateFormFields(member);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Cancels the current operation and closes the modal navigation store.
        /// </summary>
        private void ExecuteCancelRecord() => modalNavigationStore.Close();

        /// <summary>
        ///     Prompts the user for confirmation and deletes the record if confirmed.
        /// </summary>
        /// <param name="arg" >The cancellation token for the operation.</param>
        private async Task ExecuteDeleteRecord( CancellationToken arg )
        {
            MessageBoxResult confirm = messageService.Show("Are you sure you want to delete the record?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.OK)
                await OnDeleteConfirm(arg);
        }

        /// <summary>
        ///     Deletes the member record and closes the modal navigation store.
        /// </summary>
        /// <param name="cancellationToken" ></param>
        private async Task OnDeleteConfirm( CancellationToken cancellationToken )
        {
            await memberStore.DeleteMemberAsync(_memberId, cancellationToken);
            modalNavigationStore.Close();
        }

        /// <summary>
        ///     Populates the form fields with the data from the specified member.
        /// </summary>
        /// <param name="member" >The member whose data is used to populate the form fields.</param>
        private void PopulateFormFields( Member member )
        {
            _firstName = member.FirstName;
            _lastName = member.LastName;
            _contact = member.Contact;
            _address = member.Address;
            _gender = member.Gender;
            _isLeader = member.IsLeader;
            _organization = member.Organization;
            _bibleClass = member.BibleClass;
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

        #region Properties

        /// <summary>
        ///     Gets or sets the address of the member.
        /// </summary>
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        /// <summary>
        ///     Gets or sets the Bible class of the member.
        /// </summary>
        public BibleClassEnum BibleClass
        {
            get => _bibleClass;
            set => SetProperty(ref _bibleClass, value);
        }

        /// <summary>
        ///     Gets or sets the contact information of the member.
        /// </summary>
        public string Contact
        {
            get => _contact;
            set => SetProperty(ref _contact, value);
        }

        /// <summary>
        ///     Gets or sets the first name of the member.
        /// </summary>
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        /// <summary>
        ///     Gets or sets the gender of the member.
        /// </summary>
        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the member is a leader.
        /// </summary>
        public bool IsLeader
        {
            get => _isLeader;
            set => SetProperty(ref _isLeader, value);
        }

        /// <summary>
        ///     Gets or sets the last name of the member.
        /// </summary>
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        /// <summary>
        ///     Gets or sets the organization of the member.
        /// </summary>
        public OrganizationEnum Organization
        {
            get => _organization;
            set => SetProperty(ref _organization, value);
        }

        /// <summary>
        ///     Gets the available organizations for selection.
        /// </summary>
        public IEnumerable<OrganizationEnum> AvailableOrganizations { get; } = Enum.GetValues<OrganizationEnum>();

        /// <summary>
        ///     Gets the available Bible classes for selection.
        /// </summary>
        public IEnumerable<BibleClassEnum> AvailableBibleClasses { get; } = Enum.GetValues<BibleClassEnum>();

        #endregion
    }
}
