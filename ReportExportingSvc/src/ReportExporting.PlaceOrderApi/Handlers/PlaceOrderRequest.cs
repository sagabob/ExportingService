using MediatR;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Handlers
{
    public class PlaceOrderRequest: IRequest<ReportRequest>
    {
        public ReportRequest  PayLoad { get; set; }
    }
}
