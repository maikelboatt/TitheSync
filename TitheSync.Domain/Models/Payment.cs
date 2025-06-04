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
    public class Payment( int paymentId, int paymentMemberId, decimal amount, DateOnly datePaid )
    {
        /// <summary>
        ///     Gets the unique identifier for the payment.
        /// </summary>
        public int PaymentId { get; init; } = paymentId;

        /// <summary>
        ///     Gets the unique identifier for the member associated with the payment.
        /// </summary>
        public int PaymentMemberId { get; init; } = paymentMemberId;

        /// <summary>
        ///     Gets the amount of the payment.
        /// </summary>
        public decimal Amount { get; init; } = amount;

        /// <summary>
        ///     Gets the date the payment was made.
        /// </summary>
        public DateOnly DatePaid { get; init; } = datePaid;

        /// <summary>
        ///     Returns a string that represents the current <see cref="Payment" />.
        /// </summary>
        public override string ToString() => $"{nameof(PaymentId)}: {PaymentId},{nameof(PaymentMemberId)}: {PaymentMemberId}, {nameof(Amount)}: {Amount}, {nameof(DatePaid)}: {DatePaid}";

        /// <summary>
        ///     Determines whether the specified object is equal to the current <see cref="Payment" />.
        /// </summary>
        /// <param name="obj" >The object to compare with the current <see cref="Payment" />.</param>
        /// <returns><c>true</c> if the specified object is equal to the current <see cref="Payment" />; otherwise, <c>false</c>.</returns>
        public override bool Equals( object? obj )
        {
            if (obj is not Payment other)
            {
                return false;
            }

            return PaymentId == other.PaymentId;
        }

        /// <summary>
        ///     Returns a hash code for the current <see cref="Payment" />.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Payment" />.</returns>
        public override int GetHashCode() => PaymentId.GetHashCode();
    }
}
