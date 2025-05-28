namespace TitheSync.Domain.Models
{
    /// <summary>
    ///     Represents a payment with additional name information.
    /// </summary>
    /// <param name="paymentId" >The unique identifier for the payment.</param>
    /// <param name="paymentMemberId" >The unique identifier for the member associated with the payment.</param>
    /// <param name="amount" >The amount of the payment.</param>
    /// <param name="datePaid" >The date the payment was made.</param>
    /// <param name="firstName" >The first name of the member associated with the payment.</param>
    /// <param name="LastName" >The last name of the member associated with the payment.</param>
    public class PaymentWithName(
        int paymentId,
        int paymentMemberId,
        decimal amount,
        DateOnly datePaid,
        string firstName,
        string LastName ):
        Payment(paymentId, paymentMemberId, amount, datePaid)
    {
        /// <summary>
        ///     Gets the first name of the member associated with the payment.
        /// </summary>
        public string FirstName { get; init; } = firstName;

        /// <summary>
        ///     Gets the last name of the member associated with the payment.
        /// </summary>
        public string LastName { get; init; } = LastName;

        public override string ToString() =>
            $"{nameof(PaymentId)}: {PaymentId}, {nameof(PaymentMemberId)}: {PaymentMemberId}, {nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, {nameof(Amount)}: {Amount}, {nameof(DatePaid)}: {DatePaid}";
    }
}
