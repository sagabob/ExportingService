using ReportExporting.ApplicationLib.Entities;
using SendGrid.Helpers.Mail;

namespace ReportExporting.NotificationApi.Services
{
    public interface IEmailService
    {
        Task<ReportRequestObject> SendingEmailToAdminAsync(ReportRequestObject reportRequestObject);

        Task<ReportRequestObject> SendingEmailToClientAsync(ReportRequestObject reportRequestObject,
            Stream fileStream);

    }
}
