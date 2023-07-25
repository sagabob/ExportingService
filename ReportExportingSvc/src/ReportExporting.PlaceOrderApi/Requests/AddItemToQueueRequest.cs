using MediatR;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Requests
{
    public class AddItemToQueueRequest : IRequest<ReportRequest>
    {
        public ReportRequest PayLoad { get; set; }
    }
}
