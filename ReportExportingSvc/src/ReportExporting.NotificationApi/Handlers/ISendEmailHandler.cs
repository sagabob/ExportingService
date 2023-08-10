using ReportExporting.ApplicationLib.Entities;
using SendGrid;

namespace ReportExporting.NotificationApi.Handlers;

public interface ISendEmailHandler
{
    Task<ReportRequestObject> HandleSendingEmailToAdmin(ReportRequestObject reportRequestObject);
    Task<ReportRequestObject> HandleSendingEmailToClient(ReportRequestObject reportRequestObject);

    Task<Response> HandleSendingErrorEmailToAdmin(ReportRequestErrorObject reportRequestErrorObject);
}