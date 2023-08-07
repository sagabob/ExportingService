using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ProcessOrderApi.Handlers;

public interface IHandleExportProcess
{
    Task<ReportRequestObject> Handle(ReportRequestObject request);
}