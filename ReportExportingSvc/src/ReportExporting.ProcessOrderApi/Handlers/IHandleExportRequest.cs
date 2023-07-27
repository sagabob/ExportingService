using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ProcessOrderApi.Handlers
{
    public interface IHandleExportRequest
    {
        void ProcessExportRequest(ReportRequestObject request);
    }
}
