using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Collections;
using System.ComponentModel;
using TitheSync.Core.Models;
using TitheSync.Core.Stores;
using TitheSync.Core.Validation;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;

namespace TitheSync.Core.ViewModels.Members
{
    /// <summary>
    ///     ViewModel for creating a new member record.
    ///     Implements <see cref="MvxViewModel{T}" />, <see cref="IMemberCreateFormViewModel" />, and
    ///     <see cref="INotifyDataErrorInfo" />.
    /// </summary>
    public class MemberCreateFormViewModel:MvxViewModel<int>, IMemberCreateFormViewModel, INotifyDataErrorInfo
    {
        private readonly ILogger<MemberCreateFormViewModel> _logger;
        private readonly IMemberStore _memberStore;
        private readonly IModalNavigationStore _modalNavigationStore;
        private readonly INotificationStore _notificationStore;
        private readonly MemberRecordValidation _validator = new();

        private string _address = string.Empty;
        private BibleClassEnum _bibleClass;
        private string _contact = string.Empty;
        private string _firstName = string.Empty;
        private string _gender = string.Empty;
        private bool _isLeader;
        private string _lastName = string.Empty;
        private OrganizationEnum _organization;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberCreateFormViewModel" /> class.
        /// </summary>
        /// <param name="modalNavigationStore" >The modal navigation store for managing navigation.</param>
        /// <param name="memberStore" >The member store for managing member data.</param>
        /// <param name="logger" >The logger for logging information.</param>
        /// <param name="notificationStore" >The notification store for managing notifications.</param>
        public MemberCreateFormViewModel( IModalNavigationStore modalNavigationStore, IMemberStore memberStore, ILogger<MemberCreateFormViewModel> logger,
            INotificationStore notificationStore )
        {
            _modalNavigationStore = modalNavigationStore;
            _memberStore = memberStore;
            _logger = logger;
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
        ///     Prepares the ViewModel with the given parameter.
        /// </summary>
        /// <param name="parameter" >The parameter to prepare the ViewModel with.</param>
        public override void Prepare( int parameter )
        {
            _logger.LogInformation("Preparing MemberCreateFormViewModel with parameter: {Parameter}", parameter);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Cancels the record creation and closes the modal.
        /// </summary>
        private void ExecuteCancelRecord() => _modalNavigationStore.Close();

        /// <summary>
        ///     Submits the member record after validating the fields.
        /// </summary>
        /// <param name="cancellationToken" >The cancellation token for the operation.</param>
        private async Task ExecuteSubmitRecord( CancellationToken cancellationToken )
        {
            if (_validator.HasErrors) return;

            Member member = GetMemberFromFields();
            await _memberStore.AddMemberAsync(member, cancellationToken);
            _notificationStore.AddNotification(
                new Notification
                {
                    Title = "Member Created",
                    Message = $"Member {member.FirstName} {member.LastName} has been successfully created.",
                    TimeStamp = DateTime.UtcNow,
                    Type = "Success"
                });
            _modalNavigationStore.Close();
        }

        /// <summary>
        ///     Determines whether the submit command can execute.
        /// </summary>
        /// <returns>True if the form is valid; otherwise, false.</returns>
        private bool CanSubmit()
        {
            bool noFieldsEmpty = !string.IsNullOrWhiteSpace(FirstName) &&
                                 !string.IsNullOrWhiteSpace(LastName) &&
                                 !string.IsNullOrWhiteSpace(Contact) &&
                                 !string.IsNullOrWhiteSpace(Gender) &&
                                 !string.IsNullOrWhiteSpace(Address);

            return noFieldsEmpty && !HasErrors;
        }

        /// <summary>
        ///     Creates a <see cref="Member" /> object from the current form fields.
        /// </summary>
        /// <returns>A new <see cref="Member" /> object.</returns>
        private Member GetMemberFromFields() => new(
            1,
            FirstName,
            LastName,
            Contact,
            Gender,
            IsLeader,
            Address,
            Organization,
            BibleClass);

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

        #region Commands

        /// <summary>
        ///     Gets the command to submit the member record.
        /// </summary>
        public IMvxAsyncCommand SubmitRecordCommand { get; }

        /// <summary>
        ///     Gets the command to cancel the member record creation.
        /// </summary>
        public IMvxCommand CancelRecordCommand { get; }

        #endregion

        #region Properties

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
                RaisePropertyChanged();
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
