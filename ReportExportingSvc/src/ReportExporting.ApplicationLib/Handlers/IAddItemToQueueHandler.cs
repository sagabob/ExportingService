using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Handlers;

public interface IAddItemToQueueHandler
{
    Task<ReportRequestObject> Handle(ReportRequestObject request, QueueType queueType);
}