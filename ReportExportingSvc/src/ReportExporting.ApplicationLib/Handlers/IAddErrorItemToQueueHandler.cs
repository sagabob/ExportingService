using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Handlers;

public interface IAddErrorItemToQueueHandler
{
    Task Handle(ReportRequestErrorObject requestErrorObject, QueueType queueType);
}