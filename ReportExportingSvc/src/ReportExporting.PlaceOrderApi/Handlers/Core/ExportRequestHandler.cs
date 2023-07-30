using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;

namespace ReportExporting.PlaceOrderApi.Handlers.Core;

public class ExportRequestHandler : IExportRequestHandler
{
    private readonly IAddItemToQueueHandler _addItemToQueueHandler;
    private readonly IUpsertItemToTableHandler _upsertItemToTableHandler;


    public ExportRequestHandler(IAddItemToQueueHandler addItemToQueueHandler,
        IUpsertItemToTableHandler upsertItemToTableHandler)
    {
        _addItemToQueueHandler = addItemToQueueHandler;
        _upsertItemToTableHandler = upsertItemToTableHandler;
    }

    public async Task<ReportRequestObject> Handle(ReportRequestObject request)
    {
        request.Progress.Add(ExportingProgress.Submitting);
        request.Status = ExportingStatus.Ongoing;

        //place it on the Azure Table
        var result = await _upsertItemToTableHandler.Handle(request);


        //place it on the Queue for process
        result = await _addItemToQueueHandler.Handle(result, QueueType.WorkQueue);


        //update the current status with Azure table
        result = await _upsertItemToTableHandler.Handle(result);


        return result;
    }
}