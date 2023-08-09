using ReportExporting.ApplicationLib.Entities;
using SendGrid.Helpers.Mail;

namespace ReportExporting.NotificationApi.Helpers;

public interface IEmailContentHelpers
{
    Stream WrapReportRequestObjectToStream(ReportRequestObject reportRequestObject);

    Task<SendGridMessage> PrepareEmailContentForAdmin(ReportRequestObject reportRequestObject,
        Stream exportedFileStream, string? fromEmail, string? fromName, string? toEmail);

    Task<SendGridMessage> PrepareEmailContentForClient(ReportRequestObject reportRequestObject,
        Stream exportedFileStream, string? fromEmail, string? fromName);
}