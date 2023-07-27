using ReportExporting.Core;

namespace ReportExporting.ExportApi.Models;

public class ExportObjectFactory
{
    public static ExportObject CreateExportObject(ReportRequest reportRequest)
    {
        return new ExportObject
        {
            Urls = reportRequest.Urls,
            Id = reportRequest.Guid.ToString() ?? string.Empty,
            Product = reportRequest.Product,
            Format = reportRequest.Format
        };
    }
}