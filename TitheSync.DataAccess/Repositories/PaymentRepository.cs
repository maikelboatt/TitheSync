using TitheSync.DataAccess.DatabaseAccess;
using TitheSync.DataAccess.DTO;
using TitheSync.Domain.Models;
using TitheSync.Domain.Repositories;
using TitheSync.Domain.Services;

namespace TitheSync.DataAccess.Repositories
{
    /// <summary>
    ///     Repository for managing payment-related data access operations.
    /// </summary>
    /// <param name="dataAccess" >The SQL data access service.</param>
    /// <param name="dateConverterService" >The service for converting dates.</param>
    public class PaymentRepository( ISqlDataAccess dataAccess, IDateConverterService dateConverterService ):IPaymentRepository
    {
        /// <summary>
        ///     Retrieves all payments from the database.
        /// </summary>
        /// <returns>A collection of payments.</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsAsync()
        {
            // Executes a query to retrieve all payments.
            IEnumerable<PaymentDto> enumerable = await dataAccess.QueryAsync<PaymentDto, dynamic>(
                "sp.Payment_GetAll",
                new
                {
                });

            // Converts a collection of PaymentDto objects to a collection of Payment objects.
            return enumerable.Select(x => new Payment(
                                         x.PaymentId,
                                         x.PaymentMemberId,
                                         x.Amount,
                                         dateConverterService.ConvertToDateOnly(x.DatePaid)
                                     ));
        }

        /// <summary>
        ///     Retrieves a payment by its unique identifier.
        /// </summary>
        /// <param name="paymentId" >The unique identifier of the payment.</param>
        /// <returns>The payment if found; otherwise, null.</returns>
        public async Task<Payment?> GetPaymentByIdAsync( int paymentId )
        {
            // Executes a query to retrieve a payment by its unique identifier.
            IEnumerable<PaymentDto> enumerable = await dataAccess.QueryAsync<PaymentDto, dynamic>(
                "sp.Payment_GetById",
                new
                {
                    PaymentId = paymentId
                });

            // Retrieves the first payment DTO from the enumerable, or null if none exist.
            PaymentDto? paymentDto = enumerable.FirstOrDefault();

            // Converts a PaymentDto object to a Payment object if the PaymentDto is not null.
            return paymentDto is not null
                ? new Payment(
                    paymentDto.PaymentId,
                    paymentDto.PaymentMemberId,
                    paymentDto.Amount,
                    dateConverterService.ConvertToDateOnly(paymentDto.DatePaid)
                )
                : null;
        }

        /// <summary>
        ///     Adds a new payment to the database.
        /// </summary>
        /// <param name="payment" >The payment to add.</param>
        public async Task AddPaymentAsync( Payment payment )
        {
            // Creates a new instance of the PaymentDto class using the provided Payment object.
            PaymentDto record = new(
                payment.PaymentId,
                payment.PaymentMemberId,
                payment.Amount,
                dateConverterService.ConvertToDateTime(payment.DatePaid)
            );

            await dataAccess.CommandAsync(
                "sp.Payment_Add",
                new
                {
                    record.Amount,
                    record.DatePaid,
                    record.PaymentMemberId
                });
        }

        /// <summary>
        ///     Updates an existing payment in the database.
        /// </summary>
        /// <param name="payment" >The payment to update.</param>
        public async Task UpdatePaymentAsync( Payment payment )
        {
            // Creates a new instance of the PaymentDto class using the provided Payment object.
            PaymentDto record = new(
                payment.PaymentMemberId,
                payment.PaymentId,
                payment.Amount,
                dateConverterService.ConvertToDateTime(payment.DatePaid)
            );

            await dataAccess.CommandAsync(
                "sp.Payment_Update",
                record);
        }

        /// <summary>
        ///     Deletes a payment from the database by its unique identifier.
        /// </summary>
        /// <param name="paymentId" >The unique identifier of the payment to delete.</param>
        public async Task DeletePaymentAsync( int paymentId )
        {
            await dataAccess.CommandAsync(
                "sp.Payment_Delete",
                new
                {
                    PaymentId = paymentId
                });
        }
    }
}
