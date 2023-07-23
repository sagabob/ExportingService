using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Services;

namespace ReportExporting.PlaceOrderApi.Requests
{
    public class AddItemToTableRequest : IRequest<ReportRequestEntity>
    {
        public ReportRequestEntity PayLoad { get; set; }
    }
}
