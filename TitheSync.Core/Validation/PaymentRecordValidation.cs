using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TitheSync.Domain.Models;

namespace TitheSync.Core.Validation
{
    /// <summary>
    ///     Provides validation logic for payment records and implements the INotifyDataErrorInfo interface.
    /// </summary>
    public class PaymentRecordValidation:INotifyDataErrorInfo
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
                case nameof(Payment.Amount):
                    ValidateDecimalProperty((decimal)value, propertyName, "Amount must be greater than 0.");
                    break;
                case nameof(Payment.DatePaid):
                    ValidateDateProperty((DateOnly)value, propertyName, "DatePaid must be a valid date.");
                    break;
            }
        }

        private void ValidateDecimalProperty( decimal value, string propertyName, string errorMessage )
        {
            ClearError(propertyName);
            if (value <= 0)
            {
                AddError(propertyName, errorMessage);
            }
        }

        private void ValidateDateProperty( DateOnly value, string propertyName, string errorMessage )
        {
            ClearError(propertyName);
            if (value == default)
            {
                AddError(propertyName, errorMessage);
            }
        }

        #endregion

        #region Error Mutation

        private void AddError( string propertyName, string error )
        {
            if (!Errors.ContainsKey(propertyName))
                Errors[propertyName] = new List<string>();

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
