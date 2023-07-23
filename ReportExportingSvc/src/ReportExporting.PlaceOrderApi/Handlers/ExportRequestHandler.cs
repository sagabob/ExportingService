using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Requests;

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
