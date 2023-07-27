using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ProcessOrderApi.Handlers
{
    public interface IHandleExportRequest
    {
        Task<Stream?> ProcessExportRequest(ReportRequestObject request);
    }
}
