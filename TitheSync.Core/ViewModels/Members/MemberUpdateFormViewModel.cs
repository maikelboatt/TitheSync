using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections;
using System.ComponentModel;
using TitheSync.Core.Stores;
using TitheSync.Core.Validation;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;

namespace TitheSync.Core.ViewModels.Members
{
    /// <summary>
    ///     ViewModel for updating a member's information in the application.
    /// </summary>
    public class MemberUpdateFormViewModel:MvxViewModel<int>, IMemberUpdateFormViewModel, INotifyDataErrorInfo
    {
        private readonly ILogger<MemberCreateFormViewModel> _logger;
        private readonly IMemberStore _memberStore;
        private readonly IModalNavigationStore _modalNavigationStore;
        private readonly MemberRecordValidation _validator = new();
        private string _address = string.Empty;
        private BibleClassEnum _bibleClass;
        private string _contact = string.Empty;
        private string _firstName = string.Empty;
        private string _gender = string.Empty;
        private bool _isLeader;
        private string _lastName = string.Empty;

        private int _memberId;
        private OrganizationEnum _organization;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberUpdateFormViewModel" /> class.
        /// </summary>
        /// <param name="modalNavigationStore" >The modal navigation store for managing navigation.</param>
        /// <param name="memberStore" >The member store for accessing member data.</param>
        /// <param name="logger" >The logger for logging information and errors.</param>
        public MemberUpdateFormViewModel( IModalNavigationStore modalNavigationStore, IMemberStore memberStore, ILogger<MemberCreateFormViewModel> logger )
        {
            _modalNavigationStore = modalNavigationStore;
            _memberStore = memberStore;
            _logger = logger;

            _validator.ErrorsChanged += ValidatorOnErrorsChanged;

            // Initialize commands
            SubmitRecordCommand = new MvxAsyncCommand(ExecuteSubmitRecord, CanSubmit);
            CancelRecordCommand = new MvxCommand(ExecuteCancelRecord);
        }

        /// <summary>
        ///     Handles the event when validation errors change.
        /// </summary>
        /// <param name="sender" >The source of the event.</param>
        /// <param name="e" >The event data.</param>
        private void ValidatorOnErrorsChanged( object? sender, DataErrorsChangedEventArgs e )
        {
            RaisePropertyChanged(nameof(HasErrors));
            RaisePropertyChanged(nameof(CanSubmit));
        }

        #region LifeCycle

        /// <summary>
        ///     Prepares the ViewModel with the specified parameter.
        /// </summary>
        /// <param name="parameter" >The member ID to prepare the ViewModel with.</param>
        public override void Prepare( int parameter )
        {
            _logger.LogInformation("Preparing MemberUpdateFormViewModel with parameter: {Parameter}", parameter);
            _memberId = parameter;
        }

        /// <summary>
        ///     Initializes the ViewModel asynchronously.
        /// </summary>
        public override async Task Initialize()
        {
            await base.Initialize();
            Member? member = _memberStore.Members.FirstOrDefault(m => m.MemberId == _memberId);
            if (member == null)
            {
                _logger.LogError("Member with ID {MemberId} not found", _memberId);
                return;
            }
            PopulateFormFields(member);
        }

        #endregion

        #region Validation

        /// <summary>
        ///     Gets the validation errors for a specific property.
        /// </summary>
        /// <param name="propertyName" >The name of the property to get errors for.</param>
        /// <returns>An enumerable of validation errors.</returns>
        public IEnumerable GetErrors( string propertyName ) => _validator.GetErrors(propertyName);

        /// <summary>
        ///     Gets a value indicating whether the form has validation errors.
        /// </summary>
        public bool HasErrors => _validator.HasErrors;

        /// <summary>
        ///     Occurs when validation errors change.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => _validator.ErrorsChanged += value;
            remove => _validator.ErrorsChanged -= value;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Cancels the record update and closes the modal.
        /// </summary>
        private void ExecuteCancelRecord() => _modalNavigationStore.Close();

        /// <summary>
        ///     Submits the updated member record asynchronously.
        /// </summary>
        /// <param name="arg" >The cancellation token for the operation.</param>
        private async Task ExecuteSubmitRecord( CancellationToken arg )
        {
            if (_validator.HasErrors) return;

            Member member = GetMemberFromFields();
            await _memberStore.UpdateMemberAsync(member, arg);
            _modalNavigationStore.Close();
        }

        /// <summary>
        ///     Populates the form fields with the data from the specified member.
        /// </summary>
        /// <param name="member" >The member to populate the form fields with.</param>
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

        /// <summary>
        ///     Creates a new member object from the current form fields.
        /// </summary>
        /// <returns>A new <see cref="Member" /> object.</returns>
        private Member GetMemberFromFields() => new(
            _memberId,
            FirstName,
            LastName,
            Contact,
            Gender,
            IsLeader,
            Address,
            Organization,
            BibleClass);

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

        #region Properties

        /// <summary>
        ///     Determines whether the form can be submitted.
        /// </summary>
        /// <returns>True if the form can be submitted; otherwise, false.</returns>
        private bool CanSubmit() => !_validator.HasErrors;

        /// <summary>
        ///     Gets or sets the address of the member.
        /// </summary>
        public string Address
        {
            get => _address;
            set
            {
                if (value == _address) return;
                _address = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(_address);
                RaisePropertyChanged(() => Address);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the Bible class of the member.
        /// </summary>
        public BibleClassEnum BibleClass
        {
            get => _bibleClass;
            set
            {
                if (value == _bibleClass) return;
                _bibleClass = value;
                _validator.Validate(_bibleClass);
                RaisePropertyChanged(() => BibleClass);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the contact information of the member.
        /// </summary>
        public string Contact
        {
            get => _contact;
            set
            {
                if (value == _contact) return;
                _contact = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(_contact);
                RaisePropertyChanged(() => Contact);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the first name of the member.
        /// </summary>
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (value == _firstName) return;
                _firstName = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(_firstName);
                RaisePropertyChanged(() => FirstName);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the gender of the member.
        /// </summary>
        public string Gender
        {
            get => _gender;
            set
            {
                if (value == _gender) return;
                _gender = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(_gender);
                RaisePropertyChanged(() => Gender);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the member is a leader.
        /// </summary>
        public bool IsLeader
        {
            get => _isLeader;
            set
            {
                if (value == _isLeader) return;
                _isLeader = value;
                _validator.Validate(_isLeader);
                RaisePropertyChanged(() => IsLeader);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the last name of the member.
        /// </summary>
        public string LastName
        {
            get => _lastName;
            set
            {
                if (value == _lastName) return;
                _lastName = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(_lastName);
                RaisePropertyChanged(() => LastName);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the organization of the member.
        /// </summary>
        public OrganizationEnum Organization
        {
            get => _organization;
            set
            {
                if (value == _organization) return;
                _organization = value;
                _validator.Validate(_organization);
                RaisePropertyChanged(() => Organization);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
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
