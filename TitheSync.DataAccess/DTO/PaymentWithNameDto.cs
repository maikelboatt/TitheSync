namespace TitheSync.DataAccess.DTO
{
    /// <summary>
    ///     Represents a data transfer object for a payment.
    /// </summary>
    public record PaymentWithNameDto
    {
        // Parameterless constructor for materialization
        public PaymentWithNameDto() { }

        /// <summary>
        ///     Primary Constructor
        /// </summary>
        /// <param name="paymentId" >The unique identifier for the payment.</param>
        /// <param name="paymentMemberId" >
        ///     The unique identifier for the member associated with the payment.
        /// </param>
        /// <param name="amount" >The amount of the payment.</param>
        /// <param name="datePaid" >The date the payment was made.</param>
        /// <param name="firstName" >The first name of the member associated with the payment</param>
        /// <param name="lastName" >The last name of the member associated with the payment</param>
        public PaymentWithNameDto( int paymentId, int paymentMemberId, decimal amount, DateTime datePaid, string firstName, string lastName )
        {
            PaymentId = paymentId;
            PaymentMemberId = paymentMemberId;
            Amount = amount;
            DatePaid = datePaid;
            FirstName = firstName;
            LastName = lastName;
        }

        public int PaymentId { get; init; }
        public int PaymentMemberId { get; init; }
        public decimal Amount { get; init; }
        public DateTime DatePaid { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }
    }
}
