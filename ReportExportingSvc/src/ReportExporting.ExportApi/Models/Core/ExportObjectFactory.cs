using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ExportApi.Models.Core;

public class ExportObjectFactory : IExportObjectFactory
{
    public ExportObject CreateExportObject(ReportRequestObject reportRequest)
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