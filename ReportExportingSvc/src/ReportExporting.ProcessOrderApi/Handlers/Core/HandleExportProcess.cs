using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ExportApi.Handlers;

namespace ReportExporting.ProcessOrderApi.Handlers.Core;

public class HandleExportProcess : IHandleExportProcess
{
    private readonly IAddItemToQueueHandler _addItemToQueueHandler;
    private readonly IExportRequestHandler _exportRequestHandler;
    private readonly IUploadItemToBlobHandler _uploadItemToBlobHandler;
    private readonly IUpsertItemToTableHandler _upsertItemToTableHandler;


    public HandleExportProcess(IExportRequestHandler exportRequestHandler,
        IUploadItemToBlobHandler uploadItemToBlobHandler, IUpsertItemToTableHandler upsertItemToTableHandler,
        IAddItemToQueueHandler addItemToQueueHandler)
    {
        _exportRequestHandler = exportRequestHandler;
        _uploadItemToBlobHandler = uploadItemToBlobHandler;
        _upsertItemToTableHandler = upsertItemToTableHandler;
        _addItemToQueueHandler = addItemToQueueHandler;
    }

    public async Task<ReportRequestObject> Handle(ReportRequestObject request)
    {
        request.Progress.Add(ExportingProgress.DoExportingOnOrder);

        // update record
        var result = await _upsertItemToTableHandler.Handle(request);

        // do export 
        var exportFileStream = await _exportRequestHandler.ProcessExportRequest(result);

        // update record
        result = await _upsertItemToTableHandler.Handle(result);

        // upload file
        if (exportFileStream != null)
            result = await _uploadItemToBlobHandler.Handle(exportFileStream, result);

        result = await _addItemToQueueHandler.Handle(result, QueueType.EmailQueue);

        result = await _upsertItemToTableHandler.Handle(result);

        return result;
    }
}