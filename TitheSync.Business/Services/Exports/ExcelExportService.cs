using ClosedXML.Excel;
using TitheSync.Infrastructure.Models;

namespace TitheSync.Business.Services.Exports
{
    /// <summary>
    ///     Service for exporting reports to Excel files using ClosedXML.
    /// </summary>
    public class ExcelExportService:IExcelExportService
    {
        /// <summary>
        ///     Exports a collection of <see cref="MemberPaymentReport" /> to an Excel file.
        /// </summary>
        /// <param name="reports" >The member payment reports to export.</param>
        /// <param name="filePath" >The file path where the Excel file will be saved.</param>
        public void ExportMemberReports( IEnumerable<MemberPaymentReport> reports, string filePath )
        {
            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Member Comparison");
            worksheet.Cell(1, 1).Value = "Full Name";
            worksheet.Cell(1, 2).Value = "Total Amount";
            int row = 2;
            foreach (MemberPaymentReport r in reports)
            {
                worksheet.Cell(row, 1).Value = r.FullName;
                worksheet.Cell(row, 2).Value = r.TotalAmount;
                row++;
            }
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filePath);
        }

        /// <summary>
        ///     Exports a collection of <see cref="BibleClassPaymentReport" /> to an Excel file.
        /// </summary>
        /// <param name="reports" >The Bible class payment reports to export.</param>
        /// <param name="filePath" >The file path where the Excel file will be saved.</param>
        public void ExportBibleClassReports( IEnumerable<BibleClassPaymentReport> reports, string filePath )
        {
            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Bible Class Comparison");
            worksheet.Cell(1, 1).Value = "Class Name";
            worksheet.Cell(1, 2).Value = "Total Amount";
            int row = 2;
            foreach (BibleClassPaymentReport r in reports)
            {
                worksheet.Cell(row, 1).Value = r.ClassName.ToString();
                worksheet.Cell(row, 2).Value = r.TotalAmount;
                row++;
            }
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filePath);
        }
    }
}
