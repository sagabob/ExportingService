using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Requests;

namespace ReportExporting.PlaceOrderApi.Handlers
{
    public class AddItemToQueueHandler : IRequestHandler<AddItemToQueueRequest, ReportRequest>
    {
    }
}
