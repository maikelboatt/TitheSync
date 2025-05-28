using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TitheSync.Domain.Models;

namespace TitheSync.Core.Validation
{
    /// <summary>
    ///     Provides validation logic for member records and implements the INotifyDataErrorInfo interface.
    /// </summary>
    public class MemberRecordValidation:INotifyDataErrorInfo
    {
        /// <summary>
        ///     Stores validation errors for properties.
        /// </summary>
        public readonly Dictionary<string, List<string>> Errors = new();

        /// <summary>
        ///     Indicates whether there are any validation errors.
        /// </summary>
        public bool HasErrors => Errors.Count != 0;

        /// <summary>
        ///     Event triggered when the validation errors for a property change.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        ///     Retrieves the validation errors for a specified property.
        /// </summary>
        /// <param name="propertyName" >The name of the property to retrieve errors for.</param>
        /// <returns>An enumerable of error messages, or null if no errors exist for the property.</returns>
        public IEnumerable GetErrors( string? propertyName )
        {
            if (string.IsNullOrEmpty(propertyName) || !Errors.TryGetValue(propertyName, out List<string>? value))
                return null;

            return value;
        }

        /// <summary>
        ///     Raises the ErrorsChanged event for a specified property.
        /// </summary>
        /// <param name="propertyName" >The name of the property whose errors have changed.</param>
        private void OnErrorsChanged( string propertyName )
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #region Validation Methods

        /// <summary>
        ///     Validates the value of a specified property and updates the error collection.
        /// </summary>
        /// <param name="propertyName" >The name of the property to validate.</param>
        /// <param name="value" >The value of the property to validate.</param>
        public void Validate( object value, [CallerMemberName] string propertyName = "" )
        {

            switch (propertyName)
            {
                case nameof(Member.FirstName):
                case nameof(Member.LastName):
                case nameof(Member.Gender):
                case nameof(Member.Address):
                case nameof(Member.Contact):
                    ValidateStringProperty((string)value, propertyName, "Field cannot be empty", 3, 30);
                    break;
                case nameof(Member.Organization):
                case nameof(Member.BibleClass):
                    ValidateEnumProperty(value, propertyName);
                    break;
            }

        }

        private void ValidateStringProperty( string value, string propertyName, string errorMessage, int minLength, int maxLength )
        {
            ClearError(propertyName); // Clear previous errors for the property
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(propertyName, errorMessage);
            }
            else if (value.Length < minLength || value.Length > maxLength)
            {
                AddError(propertyName, $"{propertyName} must be between {minLength} and {maxLength} characters.");
            }
        }

        private void ValidateEnumProperty( object value, string propertyName )
        {
            ClearError(propertyName); // Clear previous errors for the property
            if (value == null || !Enum.IsDefined(value.GetType(), value))
            {
                AddError(propertyName, $"{propertyName} must be selected.");
            }
        }

        #endregion

        #region Error Mutation

        private void AddError( string propertyName, string error )
        {
            if (!Errors.ContainsKey(propertyName))
                Errors[propertyName] = [];

            Errors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }

        private void ClearError( string propertyName )
        {
            if (!Errors.ContainsKey(propertyName)) return;
            Errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }

        #endregion
    }
}
