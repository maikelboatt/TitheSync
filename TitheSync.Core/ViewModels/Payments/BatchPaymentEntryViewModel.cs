namespace TitheSync.Core.ViewModels.Payments
{
    /// <summary>
    ///     ViewModel representing a single batch payment entry for a member.
    /// </summary>
    public class BatchPaymentEntryViewModel
    {
        /// <summary>
        ///     Gets or sets the unique identifier of the member.
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        ///     Gets or sets the full name of the member.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the payment amount for the member.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
