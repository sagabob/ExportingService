using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.NotificationApi.Handlers;

public interface ISendEmailHandler
{
    Task<ReportRequestObject> HandleSendingEmailToAdmin(ReportRequestObject reportRequestObject);
    Task<ReportRequestObject> HandleSendingEmailToClient(ReportRequestObject reportRequestObject);
}