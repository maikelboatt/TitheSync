using TitheSync.Domain.Enums;

namespace TitheSync.ApplicationState.Stores.Reports
{
    /// <summary>
    ///     Provides aggregated payment reports for members and Bible classes.
    /// </summary>
    public interface IReportStore
    {
        /// <summary>
        ///     Gets or sets the total quarter payments for each member.
        /// </summary>
        IEnumerable<(string FullName, decimal TotalAmount)> QuarterPaymentsForMembers { get; set; }

        /// <summary>
        ///     Gets or sets the total semi-annual payments for each member.
        /// </summary>
        IEnumerable<(string FullName, decimal TotalAmount)> SemiAnnualPaymentsForMembers { get; set; }

        /// <summary>
        ///     Gets or sets the total annual payments for each member.
        /// </summary>
        IEnumerable<(string FullName, decimal TotalAmount)> AnnualPaymentsForMembers { get; set; }

        /// <summary>
        ///     Gets or sets the total quarter payments for each Bible class.
        /// </summary>
        IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> QuarterPaymentsForBibleClasses { get; set; }

        /// <summary>
        ///     Gets or sets the total semi-annual payments for each Bible class.
        /// </summary>
        IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> SemiAnnualPaymentsForBibleClasses { get; set; }

        /// <summary>
        ///     Gets or sets the total annual payments for each Bible class.
        /// </summary>
        IEnumerable<(BibleClassEnum BibleClass, decimal TotalAmount)> AnnualPaymentsForBibleClasses { get; set; }
    }
}
