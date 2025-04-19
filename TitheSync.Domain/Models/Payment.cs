namespace TitheSync.Domain.Models
{
    /// <summary>
    ///     Represents a payment transaction.
    /// </summary>
    /// <summary>
    ///     Initializes a new instance of the <see cref="Payment" /> class.
    /// </summary>
    /// <param name="paymentId" >The unique identifier for the payment.</param>
    /// <param name="amount" >The amount of the payment.</param>
    /// <param name="datePaid" >The date the payment was made.</param>
    public class Payment( int paymentId, decimal amount, DateOnly datePaid )
    {
        /// <summary>
        ///     Gets the unique identifier for the payment.
        /// </summary>
        public int PaymentId { get; init; } = paymentId;

        /// <summary>
        ///     Gets the amount of the payment.
        /// </summary>
        public decimal Amount { get; init; } = amount;

        /// <summary>
        ///     Gets the date the payment was made.
        /// </summary>
        public DateOnly DatePaid { get; init; } = datePaid;

        public override string ToString() => $"{nameof(PaymentId)}: {PaymentId}, {nameof(Amount)}: {Amount}, {nameof(DatePaid)}: {DatePaid}";
    }
}
