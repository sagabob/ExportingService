using MediatR;
using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Messages;

public class AddItemToQueueRequest : IRequest<ReportRequestObject>
{
    public ReportRequestObject PayLoad { get; set; }
}