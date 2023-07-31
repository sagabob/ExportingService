using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.PlaceOrderApi.Handlers;

public interface IExportRequestHandler
{
    Task<ReportRequestObject> Handle(ReportRequestObject request);
}