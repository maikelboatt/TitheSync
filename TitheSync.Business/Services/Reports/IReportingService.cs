using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Reports
{
    public interface IReportingService
    {
        Task<PaymentWithName> GetAllPaymentsForBibleClass( BibleClassEnum bibleClass, DateOnly startDate, DateOnly endDate );

        Task<PaymentWithName> GetAllPaymentsOfMember( DateOnly startDate, DateOnly endDate );
    }
}
