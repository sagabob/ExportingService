using MediatR;
using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Messages;

public class UpsertItemToTableRequest : IRequest<ReportRequestObject>
{
    public ReportRequestObject PayLoad { get; set; }
}