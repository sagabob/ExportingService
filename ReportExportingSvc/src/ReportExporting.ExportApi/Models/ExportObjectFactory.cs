using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ExportApi.Models;

public class ExportObjectFactory
{
    public static ExportObject CreateExportObject(ReportRequestObject reportRequest)
    {
        return new ExportObject
        {
            Urls = reportRequest.Urls,
            Id = reportRequest.Id.ToString(),
            Product = reportRequest.Product,
            Format = reportRequest.Format
        };
    }
}