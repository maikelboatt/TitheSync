using TitheSync.Domain.Enums;

namespace TitheSync.Business.Services.Reports
{
    /// <summary>
    ///     Provides reporting services for tithe payments, including data retrieval and summary generation.
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        ///     Retrieves data from the database asynchronously.
        /// </summary>
        Task GetDataFromDb();

        /// <summary>
        ///     Gets the total payments grouped by Bible class and period (Quarterly, SemiAnnual, Yearly).
        /// </summary>
        /// <returns>
        ///     A tuple containing three enumerables for Quarterly, SemiAnnual, and Yearly totals.
        /// </returns>
        (IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Quarterly,
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> SemiAnnual,
            IEnumerable<(BibleClassEnum BibleClass, string Period, decimal TotalAmount)> Yearly)
            GetTotalPaymentsByBibleClassPeriods();

        /// <summary>
        ///     Generates summary lines for the provided totals.
        /// </summary>
        /// <param name="totals" >A collection of tuples containing Bible class, period, and total amount.</param>
        /// <returns>An enumerable of summary strings.</returns>
        IEnumerable<string> GetTotalsSummaryLines( IEnumerable<(BibleClassEnum BibleClass, string period, decimal TotalAmount)> totals );

        /// <summary>
        ///     Retrieves the top ten payers in the current quarter based on total payment amounts.
        /// </summary>
        /// <remarks>
        ///     This method filters payments to only those made in the current quarter and year,
        ///     groups them by member, sums the total amount per member, and returns the top ten
        ///     payers along with their full names and total amounts.
        /// </remarks>
        /// <returns>
        ///     A list of tuples containing the full name and total amount paid by the top ten payers
        ///     in the current quarter.
        /// </returns>
        List<(string FullName, decimal TotalAmount)> GetTopTenPayersInCurrentQuarter();
    }
}
