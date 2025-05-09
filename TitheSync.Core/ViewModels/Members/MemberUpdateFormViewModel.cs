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
    public class MemberUpdateFormViewModel:MvxViewModel<int>, IMemberUpdateFormViewModel
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

        private void ValidatorOnErrorsChanged( object? sender, DataErrorsChangedEventArgs e )
        {
            RaisePropertyChanged(nameof(HasErrors));
            RaisePropertyChanged(nameof(CanSubmit));
        }

        private bool CanSubmit() => !_validator.HasErrors;


        public override void Prepare( int parameter )
        {
            _logger.LogInformation("Preparing MemberUpdateFormViewModel with parameter: {Parameter}", parameter);
            _memberId = parameter;
        }

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

        #region Validation

        public IEnumerable GetErrors( string propertyName ) => _validator.GetErrors(propertyName);

        public bool HasErrors => _validator.HasErrors;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => _validator.ErrorsChanged += value;
            remove => _validator.ErrorsChanged -= value;
        }

        #endregion

        #region Methods

        private void ExecuteCancelRecord() => _modalNavigationStore.Close();

        private async Task ExecuteSubmitRecord( CancellationToken arg )
        {
            ValidateFields();
            if (_validator.HasErrors) return;

            Member member = GetMemberFromFields();
            await _memberStore.AddMemberAsync(member, arg);
            _modalNavigationStore.Close();
        }

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

        private void ValidateFields()
        {
            _validator.Validate(nameof(FirstName), FirstName);
            _validator.Validate(nameof(LastName), LastName);
            _validator.Validate(nameof(Contact), Contact);
            _validator.Validate(nameof(Address), Address);
            _validator.Validate(nameof(Gender), Gender);
            _validator.Validate(nameof(Organization), Organization);
            _validator.Validate(nameof(BibleClass), BibleClass);
        }

        #endregion

        #region Commands

        public IMvxAsyncCommand SubmitRecordCommand { get; }
        public IMvxCommand CancelRecordCommand { get; }

        #endregion

        #region Properties

        public string Address
        {
            get => _address;
            set
            {
                if (value == _address) return;
                _address = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(nameof(Address), _address);
                RaisePropertyChanged(() => Address);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }
        public BibleClassEnum BibleClass
        {
            get => _bibleClass;
            set
            {
                if (value == _bibleClass) return;
                _bibleClass = value;
                _validator.Validate(nameof(BibleClass), _bibleClass);
                RaisePropertyChanged(() => BibleClass);
                SubmitRecordCommand.RaiseCanExecuteChanged();

            }
        }
        public string Contact
        {
            get => _contact;
            set
            {
                if (value == _contact) return;
                _contact = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(nameof(Contact), _contact);
                RaisePropertyChanged(() => Contact);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (value == _firstName) return;
                _firstName = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(nameof(FirstName), _firstName);
                RaisePropertyChanged(() => FirstName);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }
        public string Gender
        {
            get => _gender;
            set
            {
                if (value == _gender) return;
                _gender = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(nameof(Gender), _gender);
                RaisePropertyChanged(() => Gender);
                SubmitRecordCommand.RaiseCanExecuteChanged();

            }
        }
        public bool IsLeader
        {
            get => _isLeader;
            set
            {
                if (value == _isLeader) return;
                _isLeader = value;
                _validator.Validate(nameof(IsLeader), _isLeader);
                RaisePropertyChanged(() => IsLeader);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }
        public string LastName
        {
            get => _lastName;
            set
            {
                if (value == _lastName) return;
                _lastName = value ?? throw new ArgumentNullException(nameof(value));
                _validator.Validate(nameof(LastName), _lastName);
                RaisePropertyChanged(() => LastName);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }
        public OrganizationEnum Organization
        {
            get => _organization;
            set
            {
                if (value == _organization) return;
                _organization = value;
                _validator.Validate(nameof(Organization), _organization);
                RaisePropertyChanged(() => Organization);
                SubmitRecordCommand.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<OrganizationEnum> AvailableOrganizations { get; } = Enum.GetValues<OrganizationEnum>();
        public IEnumerable<BibleClassEnum> AvailableBibleClasses { get; } = Enum.GetValues<BibleClassEnum>();

        #endregion
    }
}
