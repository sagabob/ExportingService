using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;

namespace ReportExporting.PlaceOrderApi.Handlers.Core;

public class ExportRequestHandler(
    IAddItemToQueueHandler addItemToQueueHandler,
    IUpsertItemToTableHandler upsertItemToTableHandler)
    : IExportRequestHandler
{
    public async Task<ReportRequestObject> Handle(ReportRequestObject request)
    {
        request.Progress.Add(ExportingProgress.Submitting);

        //place it on the Azure Table
        var result = await upsertItemToTableHandler.Handle(request);

        //place it on the Queue for process
        result = await addItemToQueueHandler.Handle(result, QueueType.WorkQueue);


        //update the current status with Azure table
        result = await upsertItemToTableHandler.Handle(result);

        if (result.Status == ExportingStatus.Failure)
            //TODO will implement a backup notification to notify admin if email queue fails
            result = await addItemToQueueHandler.Handle(result, QueueType.EmailQueue);


        return result;
    }
}