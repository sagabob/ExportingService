using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Requests;
using ReportExporting.PlaceOrderApi.Services;

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
        var tableEntity =
            await _mediator.Send(new AddItemToTableRequest { PayLoad = new ReportRequestEntity(request.PayLoad) },
                cancellationToken);
        return request.PayLoad;
    }
}