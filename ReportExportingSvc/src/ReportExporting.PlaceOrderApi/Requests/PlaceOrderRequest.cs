using MediatR;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Requests;

public class PlaceOrderRequest : IRequest<ReportRequest>
{
    public ReportRequest PayLoad { get; set; }
}