using ReportExporting.ApplicationLib.Entities;
using SendGrid.Helpers.Mail;

namespace ReportExporting.NotificationApi.Helpers;

public class EmailContentHelpers
{
    public static async Task<SendGridMessage> PrepareEmailContentForAdmin(ReportRequestObject reportRequestObject,
        Stream exportedFileStream, string? fromEmail, string? fromName, string? toEmail)
    {
        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromEmail, fromName),
            Subject = $"Notify on failure of report {reportRequestObject.FileName}",
            HtmlContent = "<h3>Here is the order detail & error</h3>"
        };
        msg.AddTo(toEmail);
        await msg.AddAttachmentAsync(reportRequestObject.FileName, exportedFileStream);

        return msg;
    }

    public static async Task<SendGridMessage> PrepareEmailContentForClient(ReportRequestObject reportRequestObject,
        Stream exportedFileStream, string? fromEmail, string? fromName)
    {
        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromEmail, fromName),
            Subject = $"Your ordered report {reportRequestObject.FileName}",
            HtmlContent = "<h3>Thank you for using our service, please see the attachment</h3>"
        };

        msg.AddTo(reportRequestObject.EmailAddress);
        await msg.AddAttachmentAsync($"{reportRequestObject.FileName}.json", exportedFileStream);

        return msg;
    }
}