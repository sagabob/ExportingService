using System.Text;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using SendGrid.Helpers.Mail;

namespace ReportExporting.NotificationApi.Helpers.Core;

public class EmailContentHelpers : IEmailContentHelpers
{
    public Stream WrapReportRequestObjectToStream(ReportRequestObject reportRequestObject)
    {
        var jsonString = JsonConvert.SerializeObject(reportRequestObject);
        var byteArray = Encoding.ASCII.GetBytes(jsonString);
        var stream = new MemoryStream(byteArray)
        {
            ReadTimeout = 0,
            WriteTimeout = 0,
            Capacity = 0,
            Position = 0
        };

        return stream;
    }

    public async Task<SendGridMessage> PrepareEmailContentForAdmin(ReportRequestObject reportRequestObject,
        Stream exportedFileStream, string? fromEmail, string? fromName, string? toEmail)
    {
        var msg = CreateMessageForAdmin(reportRequestObject, fromEmail, fromName);
        msg.AddTo(toEmail);
        await msg.AddAttachmentAsync($"Request-{reportRequestObject.FileName}.json", exportedFileStream);

        return msg;
    }

    public async Task<SendGridMessage> PrepareEmailContentForClient(ReportRequestObject reportRequestObject,
        Stream exportedFileStream, string? fromEmail, string? fromName)
    {
        var msg = CreateMessageForClient(reportRequestObject, fromEmail, fromName);

        msg.AddTo(reportRequestObject.EmailAddress);
        await msg.AddAttachmentAsync($"{reportRequestObject.FileName}", exportedFileStream);

        return msg;
    }

    public SendGridMessage CreateMessageForClient(ReportRequestObject reportRequestObject, string? fromEmail,
        string? fromName)
    {
        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromEmail, fromName),
            Subject = $"Your ordered report {reportRequestObject.FileName}",
            PlainTextContent = "Thank you for using our service, please see the attachment"
        };

        return msg;
    }

    public SendGridMessage CreateMessageForAdmin(ReportRequestObject reportRequestObject, string? fromEmail,
        string? fromName)
    {
        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromEmail, fromName),
            Subject = $"Notify on failure of report {reportRequestObject.FileName}",
            PlainTextContent = "Here is the order detail & error"
        };

        return msg;
    }
}