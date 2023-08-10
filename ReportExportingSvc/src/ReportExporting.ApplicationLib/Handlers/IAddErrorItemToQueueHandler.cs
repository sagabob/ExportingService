using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Handlers;

public interface IAddErrorItemToQueueHandler
{
    Task<bool> Handle(ReportRequestErrorObject requestErrorObject, QueueType queueType);
}