namespace TitheSync.DataAccess.DTO
{
    /// <summary>
    ///     Represents a payment data transfer object.
    /// </summary>
    /// <param name="PaymentId" >The unique identifier for the payment.</param>
    /// <param name="Amount" >The amount of the payment.</param>
    /// <param name="DatePaid" >The date the payment was made.</param>
    public record PaymentDto(
        int PaymentId,
        decimal Amount,
        DateTime DatePaid );
}
