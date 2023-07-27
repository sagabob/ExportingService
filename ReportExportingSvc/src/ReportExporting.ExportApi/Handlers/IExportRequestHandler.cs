using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ExportApi.Handlers
{
    public interface IExportRequestHandler
    {
        Task<Stream?> ProcessExportRequest(ReportRequestObject request);
    }
}
