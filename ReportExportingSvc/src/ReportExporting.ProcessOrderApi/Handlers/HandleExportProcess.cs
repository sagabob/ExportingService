using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ExportApi.Handlers;

namespace ReportExporting.ProcessOrderApi.Handlers;

public class HandleExportProcess
{
    private readonly IExportRequestHandler _exportRequestHandler;
    private readonly IUploadItemToBlobHandler _uploadItemToBlobHandler;
    private readonly IUpsertItemToTableHandler _upsertItemToTableHandler;


    public HandleExportProcess(IExportRequestHandler exportRequestHandler,
        IUploadItemToBlobHandler uploadItemToBlobHandler, IUpsertItemToTableHandler upsertItemToTableHandler)
    {
        _exportRequestHandler = exportRequestHandler;
        _uploadItemToBlobHandler = uploadItemToBlobHandler;
        _upsertItemToTableHandler = upsertItemToTableHandler;
    }

    public void Handle(ReportRequestObject request)
    {

    }
}