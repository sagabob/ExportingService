using MediatR;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Messages;

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
        request.PayLoad.Progress.Add(ExportingProgress.Submitting);

        //place it on the Azure Table
        var result =
            await _mediator.Send(new UpsertItemToTableRequest { PayLoad = request.PayLoad },
                cancellationToken);


        //place it on the Queue for process
        result = await _mediator.Send(new AddItemToQueueRequest { PayLoad = result }, cancellationToken);


        //update the current status with Azure table
        result = await _mediator.Send(new UpsertItemToTableRequest { PayLoad = result },
            cancellationToken);


        return result;
    }
}