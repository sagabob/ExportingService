using MediatR;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Messages;

namespace ReportExporting.PlaceOrderApi.Handlers;

public class ExportRequestHandler : IRequestHandler<PlaceOrderRequest, ReportRequestObject>
{
    private readonly IAddItemToQueueHandler _addItemToQueueHandler;
    private readonly IUpsertItemToTableHandler _upsertItemToTableHandler;


    public ExportRequestHandler(IAddItemToQueueHandler addItemToQueueHandler,
        IUpsertItemToTableHandler upsertItemToTableHandler)
    {
        _addItemToQueueHandler = addItemToQueueHandler;
        _upsertItemToTableHandler = upsertItemToTableHandler;
    }

    public async Task<ReportRequestObject> Handle(PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        request.PayLoad.Progress.Add(ExportingProgress.Submitting);

        //place it on the Azure Table
        var result = await _upsertItemToTableHandler.Handle(request.PayLoad);


        //place it on the Queue for process
        result = await _addItemToQueueHandler.Handle(result, QueueType.WorkQueue);


        //update the current status with Azure table
        result = await _upsertItemToTableHandler.Handle(result);


        return result;
    }
}