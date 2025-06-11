using TitheSync.Domain.Enums;

namespace TitheSync.Business.Services.Reports
{
    /// <summary>
    ///     Provides methods to compare and retrieve payment totals by Bible class for different time periods.
    /// </summary>
    public interface IReportCompareService
    {
        /// <summary>
        ///     Gets the total payments by Bible class for a specified quarter and optional year.
        /// </summary>
        /// <param name="quarter" >The quarter (1-4) to retrieve data for.</param>
        /// <param name="year" >The year to retrieve data for. If null, uses the current year.</param>
        /// <returns>An enumerable of tuples containing the Bible class and total amount.</returns>
        IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> GetPaymentsByBibleClassForQuarter( int quarter, int? year = null );

        /// <summary>
        ///     Gets the total payments by Bible class for a specified half-year and optional year.
        /// </summary>
        /// <param name="half" >The half-year (1 or 2) to retrieve data for.</param>
        /// <param name="year" >The year to retrieve data for. If null, uses the current year.</param>
        /// <returns>An enumerable of tuples containing the Bible class and total amount.</returns>
        IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> GetPaymentsByBibleClassForHalfYear( int half, int? year = null );

        /// <summary>
        ///     Gets the total payments by Bible class for a specified year.
        /// </summary>
        /// <param name="year" >The year to retrieve data for.</param>
        /// <returns>An enumerable of tuples containing the Bible class and total amount.</returns>
        IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> GetPaymentsByBibleClassForYear( int year );

        /// <summary>
        ///     Gets the total payments by member for a specified quarter and optional year.
        /// </summary>
        /// <param name="quarter" >The quarter (1-4) to retrieve data for.</param>
        /// <param name="year" >The year to retrieve data for. If null, uses the current year.</param>
        /// <returns>An enumerable of tuples containing the member's full name and total amount.</returns>
        IEnumerable<(string FullName, decimal TotalAmount)> GetPaymentsByMemberForQuarter( int quarter, int? year = null );

        /// <summary>
        ///     Gets the total payments by member for a specified half-year and optional year.
        /// </summary>
        /// <param name="half" >The half-year (1 or 2) to retrieve data for.</param>
        /// <param name="year" >The year to retrieve data for. If null, uses the current year.</param>
        /// <returns>An enumerable of tuples containing the member's full name and total amount.</returns>
        IEnumerable<(string FullName, decimal TotalAmount)> GetPaymentsByMemberForHalfYear( int half, int? year = null );

        /// <summary>
        ///     Gets the total payments by member for a specified year.
        /// </summary>
        /// <param name="year" >The year to retrieve data for.</param>
        /// <returns>An enumerable of tuples containing the member's full name and total amount.</returns>
        IEnumerable<(string FullName, decimal TotalAmount)> GetPaymentsByMemberForYear( int year );
    }
}
