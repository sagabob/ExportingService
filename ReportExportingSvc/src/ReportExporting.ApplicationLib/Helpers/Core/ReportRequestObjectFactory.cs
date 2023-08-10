using ReportExporting.ApplicationLib.Entities;
using ReportExporting.Core;

namespace ReportExporting.ApplicationLib.Helpers.Core;

public class ReportRequestObjectFactory : IReportRequestObjectFactory
{
    public ReportRequestObject CreateFromReportRequest(ReportRequest request)
    {
        return new ReportRequestObject
        {
            Id = Guid.NewGuid().ToString(),
            EmailAddress = request.EmailAddress,
            Format = request.Format,
            Title = request.Title,
            Product = request.Product,
            Urls = request.Urls,
            Status = ExportingStatus.Ongoing
            
        };
    }
}