using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.NotificationApi.Services;

public interface IEmailService
{
    Task<ReportRequestObject> SendingEmailToAdminAsync(ReportRequestObject reportRequestObject);

    Task<ReportRequestObject> SendingEmailToClientAsync(ReportRequestObject reportRequestObject,
        Stream fileStream);
}