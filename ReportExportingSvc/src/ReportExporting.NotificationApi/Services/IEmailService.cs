using ReportExporting.ApplicationLib.Entities;
using SendGrid;

namespace ReportExporting.NotificationApi.Services;

public interface IEmailService
{
    Task<ReportRequestObject> SendingEmailToAdminAsync(ReportRequestObject reportRequestObject);

    Task<ReportRequestObject> SendingEmailToClientAsync(ReportRequestObject reportRequestObject,
        Stream fileStream);

    Task<Response> SendingEmailWithErrorToAdminAsync(ReportRequestErrorObject reportRequestErrorObject);
}