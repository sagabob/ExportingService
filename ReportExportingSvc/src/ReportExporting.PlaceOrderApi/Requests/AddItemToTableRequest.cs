using MediatR;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Requests
{
    public class AddItemToTableRequest : IRequest<ReportRequest>
    {
        public ReportRequest PayLoad { get; set; }
    }
}
