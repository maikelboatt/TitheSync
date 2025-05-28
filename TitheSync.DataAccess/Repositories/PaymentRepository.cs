using Microsoft.Extensions.Logging;
using TitheSync.DataAccess.DatabaseAccess;
using TitheSync.DataAccess.DTO;
using TitheSync.Domain.Models;
using TitheSync.Infrastructure.Services;

namespace TitheSync.DataAccess.Repositories
{
    /// <summary>
    ///     Repository for managing payment-related data access operations.
    /// </summary>
    public class PaymentRepository:IPaymentRepository
    {
        private readonly ISqlDataAccess _dataAccess;
        private readonly IDatabaseExecutionExceptionHandlingService _databaseExecutionExceptionHandlingService;
        private readonly IDateConverterService _dateConverterService;
        private readonly ILogger<PaymentRepository> _logger;

        /// <summary>
        ///     Repository for managing payment-related data access operations.
        /// </summary>
        /// <param name="dataAccess" >The SQL data access service.</param>
        /// <param name="dateConverterService" >The service for converting dates.</param>
        /// <param name="logger" >The logger instance for logging repository operations.</param>
        /// <param name="databaseExecutionExceptionHandlingService" ></param>
        public PaymentRepository( ISqlDataAccess dataAccess, IDateConverterService dateConverterService, ILogger<PaymentRepository> logger,
            IDatabaseExecutionExceptionHandlingService databaseExecutionExceptionHandlingService )
        {
            // Validate the data access layer
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _dateConverterService = dateConverterService ?? throw new ArgumentNullException(nameof(dateConverterService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseExecutionExceptionHandlingService = databaseExecutionExceptionHandlingService;
        }

        /// <summary>
        ///     Retrieves all payments from the database.
        /// </summary>
        /// <returns>A collection of <see cref="Payment" /> objects.</returns>
        /// <exception cref="Exception" >Thrown when an error occurs during data retrieval.</exception>
        public async Task<IEnumerable<Payment>> GetPaymentsAsync()
        {
            try
            {
                IEnumerable<PaymentDto> result = await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                    "sp.Payment_GetAll",
                    new { },
                    async () => await _dataAccess.QueryAsync<PaymentDto, dynamic>("sp.Payment_GetAll", new { })
                );
                return result.Select(MapToPayment);
            }
            catch (Exception e)
            {
                // Log the exception
                _logger.LogError(e, "Error retrieving payments ");
                throw;
            }
        }

        public async Task<IEnumerable<PaymentWithName>> GetPaymentsWithNamesAsync()
        {
            try
            {
                IEnumerable<PaymentWithNameDto> result = await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                    "sp.Payment_GetAllWithNames",
                    new { },
                    async () => await _dataAccess.QueryAsync<PaymentWithNameDto, dynamic>("sp.Payment_GetAllWithNames", new { })
                );
                return result.Select(MapToPaymentWithName);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving payments with names");
                throw;
            }
        }

        /// <summary>
        ///     Retrieves a payment by its unique identifier.
        /// </summary>
        /// <param name="id" >The unique identifier of the payment.</param>
        /// <returns>A <see cref="Payment" /> object if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException" >Thrown when the <paramref name="id" /> is invalid.</exception>
        /// <exception cref="Exception" >Thrown when an error occurs during data retrieval.</exception>
        public async Task<Payment?> GetPaymentByIdAsync( int id )
        {
            if (id <= 0)
                throw new ArgumentException("Invalid payment ID.", nameof(id));

            try
            {
                IEnumerable<PaymentDto> result = await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                    "sp.Payment_GetById",
                    new { PaymentId = id },
                    async () => await _dataAccess.QueryAsync<PaymentDto, dynamic>("sp.Payment_GetById", new { PaymentId = id })
                );
                return result.Select(MapToPayment).FirstOrDefault();
            }
            catch (ArgumentException e)
            {
                // Log the Argument Exception
                _logger.LogError("Invalid member Id {Id}: {ExMessage}", id, e.Message);
                throw;
            }
            catch (Exception e)
            {
                // Log the exception
                _logger.LogError(e, "Error retrieving payment with ID {Id}", id);
                throw;
            }
        }

        /// <summary>
        ///     Adds a new payment to the database.
        /// </summary>
        /// <param name="payment" >The <see cref="Payment" /> object to add.</param>
        /// <exception cref="ArgumentNullException" >Thrown when the <paramref name="payment" /> is null.</exception>
        /// <exception cref="Exception" >Thrown when an error occurs during data insertion.</exception>
        public async Task AddPaymentAsync( PaymentWithName payment )
        {
            try
            {
                ArgumentNullException.ThrowIfNull(payment);

                await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                    "sp.Payment_Add",
                    new { },
                    async () =>
                    {
                        PaymentWithNameDto record = MapToPaymentWithNameDto(payment);
                        await _dataAccess.CommandAsync(
                            "sp.Payment_Add",
                            new
                            {
                                record.Amount,
                                record.DatePaid,
                                record.PaymentMemberId
                            });
                        return true; // Dummy return for Task<bool>
                    }
                );
            }
            catch (ArgumentNullException e)
            {
                // Log the ArgumentNullException
                _logger.LogError("Invalid member data: {ExMessage}", e.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError("Error adding member: {ExMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Updates an existing payment in the database.
        /// </summary>
        /// <param name="payment" >The <see cref="Payment" /> object to update.</param>
        /// <exception cref="ArgumentNullException" >Thrown when the <paramref name="payment" /> is null.</exception>
        /// <exception cref="Exception" >Thrown when an error occurs during data update.</exception>
        public async Task UpdatePaymentAsync( PaymentWithName payment )
        {
            try
            {
                ArgumentNullException.ThrowIfNull(payment);

                await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                    "sp.Payment_Update",
                    new { },
                    async () =>
                    {
                        PaymentWithNameDto record = MapToPaymentWithNameDto(payment);
                        await _dataAccess.CommandAsync(
                            "sp.Payment_Update",
                            new
                            {
                                record.PaymentId,
                                record.Amount,
                                record.DatePaid,
                                record.PaymentMemberId
                            });
                        return true; // Dummy return for Task<bool>
                    }
                );
            }
            catch (ArgumentNullException e)
            {
                // Log the ArgumentNullException
                _logger.LogError("Invalid member data: {ExMessage}", e.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError("Error adding member: {ExMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Deletes a payment from the database by its unique identifier.
        /// </summary>
        /// <param name="id" >The unique identifier of the payment to delete.</param>
        /// <exception cref="ArgumentException" >Thrown when the <paramref name="id" /> is invalid.</exception>
        /// <exception cref="Exception" >Thrown when an error occurs during data deletion.</exception>
        public async Task DeletePaymentAsync( int id )
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Invalid payment ID.", nameof(id));

                await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                    "sp.Payment_Delete",
                    new { PaymentId = id },
                    async () =>
                    {
                        await _dataAccess.CommandAsync("sp.Payment_Delete", new { PaymentId = id });
                        return true; // Dummy return for Task<bool>
                    }
                );
            }
            catch (ArgumentException e)
            {
                // Log the ArgumentException
                _logger.LogError("Invalid member Id {Id}: {ExMessage}", id, e.Message);
                throw;
            }
            catch (Exception e)
            {
                // Log the exception
                _logger.LogError(e, "Error deleting payment with ID {Id}", id);
                throw;
            }
        }

        /// <summary>
        ///     Maps a <see cref="PaymentDto" /> object to a <see cref="Payment" /> object.
        /// </summary>
        /// <param name="dto" >The <see cref="PaymentDto" /> object to map.</param>
        /// <returns>A <see cref="Payment" /> object.</returns>
        private Payment MapToPayment( PaymentDto dto ) => new(
            dto.PaymentId,
            dto.PaymentMemberId,
            dto.Amount,
            _dateConverterService.ConvertToDateOnly(dto.DatePaid)
        );

        /// <summary>
        ///     Maps a <see cref="PaymentWithNameDto" /> object to a <see cref="PaymentWithName" /> object.
        /// </summary>
        /// <param name="dto" >The <see cref="PaymentWithNameDto" /> object to map.</param>
        /// <returns>A <see cref="PaymentWithName" /> object.</returns>
        private PaymentWithName MapToPaymentWithName( PaymentWithNameDto dto ) => new(
            dto.PaymentId,
            dto.PaymentMemberId,
            dto.Amount,
            _dateConverterService.ConvertToDateOnly(dto.DatePaid),
            dto.FirstName,
            dto.LastName
        );

        /// <summary>
        ///     Maps a <see cref="PaymentWithName" /> object to a <see cref="PaymentWithNameDto" /> object.
        /// </summary>
        /// <param name="payment" >The <see cref="PaymentWithName" /> object to map.</param>
        /// <returns>A <see cref="PaymentWithNameDto" /> object.</returns>
        private PaymentWithNameDto MapToPaymentWithNameDto( PaymentWithName payment ) => new(
            payment.PaymentId,
            payment.PaymentMemberId,
            payment.Amount,
            _dateConverterService.ConvertToDateTime(payment.DatePaid),
            payment.FirstName,
            payment.LastName
        );
    }
}
