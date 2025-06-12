using TitheSync.Infrastructure.Models;

namespace TitheSync.Business.Services.Exports
{
   /// <summary>
    /// Provides methods to export payment reports to Excel files.
    /// </summary>
    public interface IExcelExportService
    {
        /// <summary>
        /// Exports a collection of member payment reports to an Excel file.
        /// </summary>
        /// <param name="reports">The collection of member payment reports to export.</param>
        /// <param name="filePath">The file path where the Excel file will be saved.</param>
        void ExportMemberReports( IEnumerable<MemberPaymentReport> reports, string filePath );
    
        /// <summary>
        /// Exports a collection of Bible class payment reports to an Excel file.
        /// </summary>
        /// <param name="reports">The collection of Bible class payment reports to export.</param>
        /// <param name="filePath">The file path where the Excel file will be saved.</param>
        void ExportBibleClassReports( IEnumerable<BibleClassPaymentReport> reports, string filePath );
    }
}
