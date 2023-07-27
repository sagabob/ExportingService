using MediatR;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Messages;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Handlers;

public class ExportRequestHandler : IRequestHandler<PlaceOrderRequest, ReportRequestObject>
{
    private readonly IMediator _mediator;

    public ExportRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ReportRequestObject> Handle(PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        request.PayLoad.Progress.Add(Ex);

        //place it on the Azure Table
        var tableEntity =
            await _mediator.Send(new UpsertItemToTableRequest { PayLoad = ReportRequestTableEntityFactory.CreateTableEntity(request.PayLoad) },
                cancellationToken);
       
        

        //place it on the Queue for process
        var outputQueueRequest = await _mediator.Send(new AddItemToQueueRequest() { PayLoad = request.PayLoad }, cancellationToken);


        //update the current status with Azure table
        tableEntity =
            await _mediator.Send(new AddItemToTableRequest { PayLoad = DataFactory.CreateTableEntity(outputQueueRequest) },
                cancellationToken);

        request.PayLoad.Status = tableEntity.Status;
      

        return request.PayLoad;
    }
}