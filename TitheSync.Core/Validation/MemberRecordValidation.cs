using System.Collections;
using System.ComponentModel;

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
        private readonly Dictionary<string, List<string>> _errors = new();

        /// <summary>
        ///     Indicates whether there are any validation errors.
        /// </summary>
        public bool HasErrors => _errors.Count != 0;

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
            if (string.IsNullOrEmpty(propertyName) || !_errors.TryGetValue(propertyName, out List<string>? value))
                return null;

            return value;
        }

        /// <summary>
        ///     Validates the value of a specified property and updates the error collection.
        /// </summary>
        /// <param name="propertyName" >The name of the property to validate.</param>
        /// <param name="value" >The value of the property to validate.</param>
        public void Validate( string propertyName, object value )
        {
            _errors.Remove(propertyName);

            List<string> errors = [];

            switch (value)
            {
                case string strValue when string.IsNullOrWhiteSpace(strValue):
                    errors.Add($"{propertyName} cannot be empty.");
                    break;
                case null:
                    errors.Add($"{propertyName} is required.");
                    break;
            }

            if (errors.Count != 0)
                _errors[propertyName] = errors;

            OnErrorsChanged(propertyName);
        }

        /// <summary>
        ///     Raises the ErrorsChanged event for a specified property.
        /// </summary>
        /// <param name="propertyName" >The name of the property whose errors have changed.</param>
        private void OnErrorsChanged( string propertyName )
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
