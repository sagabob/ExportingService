using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Services;

namespace ReportExporting.PlaceOrderApi.Requests
{
    public class PlaceOrderRequest : IRequest<ReportRequest>
    {
        public ReportRequest PayLoad { get; set; }
    }
}
