namespace TitheSync.DataAccess.DTO
{
    /// <summary>
    ///     Represents a data transfer object for a payment.
    /// </summary>
    public record PaymentDto
    {
        // Parameterless constructor for materialization
        public PaymentDto() { }

        /// <summary>
        ///     Primary Constructor
        /// </summary>
        /// <param name="paymentId" >The unique identifier for the payment.</param>
        /// <param name="paymentMemberId" >
        ///     The unique identifier for the member associated with the payment.
        /// </param>
        /// <param name="amount" >The amount of the payment.</param>
        /// <param name="datePaid" >The date the payment was made.</param>
        public PaymentDto( int paymentId, int paymentMemberId, decimal amount, DateTime datePaid )
        {
            PaymentId = paymentId;
            PaymentMemberId = paymentMemberId;
            Amount = amount;
            DatePaid = datePaid;
        }

        public int PaymentId { get; init; }
        public int PaymentMemberId { get; init; }
        public decimal Amount { get; init; }
        public DateTime DatePaid { get; init; }
    }
}
