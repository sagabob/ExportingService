using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Helpers;
using ReportExporting.PlaceOrderApi.Requests;

namespace ReportExporting.PlaceOrderApi.Handlers;

public class ExportRequestHandler : IRequestHandler<PlaceOrderRequest, ReportRequest>
{
    private readonly IMediator _mediator;

    public ExportRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ReportRequest> Handle(PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        //place it on the Azure Table
        var tableEntity =
            await _mediator.Send(new AddItemToTableRequest { PayLoad = DataFactory.CreateTableEntity(request.PayLoad) },
                cancellationToken);
        request.PayLoad.Status = tableEntity.Status;
        request.PayLoad.Guid = new Guid(tableEntity.PartitionKey);

        //place it on the Queue for process
        var outputQueueRequest = await _mediator.Send(new AddItemToQueueRequest() { PayLoad = request.PayLoad }, cancellationToken);

        tableEntity =
            await _mediator.Send(new AddItemToTableRequest { PayLoad = DataFactory.CreateTableEntity(outputQueueRequest) },
                cancellationToken);

        request.PayLoad.Status = tableEntity.Status;
      

        return request.PayLoad;
    }
}