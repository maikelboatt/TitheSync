using TitheSync.ApplicationState.Stores.Members;
using TitheSync.ApplicationState.Stores.Payments;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Reports
{
    public class ReportingService( IMemberStore memberStore, IPaymentStore paymentStore ):IReportingService
    {
        public Task<PaymentWithName> GetAllPaymentsForBibleClass( BibleClassEnum bibleClass, DateOnly startDate, DateOnly endDate ) => throw new NotImplementedException();

        public Task<PaymentWithName> GetAllPaymentsOfMember( DateOnly startDate, DateOnly endDate ) => throw new NotImplementedException();
    }
}
