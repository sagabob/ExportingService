using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ExportApi.Handlers;

namespace ReportExporting.ProcessOrderApi.Handlers.Core;

public class HandleExportProcess(
    IExportRequestHandler exportRequestHandler,
    IUploadItemToBlobHandler uploadItemToBlobHandler,
    IUpsertItemToTableHandler upsertItemToTableHandler,
    IAddItemToQueueHandler addItemToQueueHandler)
    : IHandleExportProcess
{
    public async Task<ReportRequestObject> Handle(ReportRequestObject request)
    {
        request.Progress.Add(ExportingProgress.DoExportingOnOrder);

        // update record
        var result = await upsertItemToTableHandler.Handle(request);

        // do export 
        var exportFileStream = await exportRequestHandler.ProcessExportRequest(result);

        // update record
        result = await upsertItemToTableHandler.Handle(result);

        // upload file
        if (exportFileStream != null)
            result = await uploadItemToBlobHandler.Handle(exportFileStream, result);

        result = await addItemToQueueHandler.Handle(result, QueueType.EmailQueue);

        result = await upsertItemToTableHandler.Handle(result);

        return result;
    }
}