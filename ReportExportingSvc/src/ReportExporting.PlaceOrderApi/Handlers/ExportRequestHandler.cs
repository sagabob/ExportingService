using MediatR;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Handlers
{
    public class ExportRequestHandler : IRequestHandler<PlaceOrderRequest, ReportRequest>
    {
        public async Task<ReportRequest> Handle(PlaceOrderRequest request, CancellationToken cancellationToken)
        {
            return request.PayLoad;
        }
    }
}
