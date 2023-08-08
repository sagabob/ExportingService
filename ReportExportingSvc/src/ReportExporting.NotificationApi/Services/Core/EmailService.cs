using System.Text;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.NotificationApi.Helpers;
using SendGrid;

namespace ReportExporting.NotificationApi.Services.Core;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ISendGridClient _sendGridClient;

    public EmailService(
        ISendGridClient sendGridClient,
        IConfiguration configuration)
    {
        _sendGridClient = sendGridClient;
        _configuration = configuration;
    }


    public async Task<ReportRequestObject> SendingEmailToAdminAsync(ReportRequestObject reportRequestObject)
    {
        try
        {
            var fromEmail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromEmail");

            var fromName = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromName");

            var toEmail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("AdminEmail");

            //email to admin
            var jsonString = JsonConvert.SerializeObject(reportRequestObject);
            var byteArray = Encoding.ASCII.GetBytes(jsonString);
            var stream = new MemoryStream(byteArray)
            {
                ReadTimeout = 0,
                WriteTimeout = 0,
                Capacity = 0,
                Position = 0
            };
            var msg = await EmailContentHelpers.PrepareEmailContentForAdmin(reportRequestObject, stream, fromEmail,
                fromName, toEmail);

            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                reportRequestObject.Progress.Add(ExportingProgress.SendEmailToAdmin);
            }
            else
            {
                reportRequestObject.Progress.Add(ExportingProgress.FailSendingEmailToAdmin);
                reportRequestObject.ErrorMessage = response.Body.ToString();
            }
        }
        catch (Exception ex)
        {
            reportRequestObject.Progress.Add(ExportingProgress.FailSendingEmailToAdmin);
            reportRequestObject.ErrorMessage = ex.Message;
            reportRequestObject.Status = ExportingStatus.Failure;
        }
        //email to admin

        return reportRequestObject;
    }

    public async Task<ReportRequestObject> SendingEmailToClientAsync(ReportRequestObject reportRequestObject,
        Stream fileStream)
    {
        try
        {
            var fromEmail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromEmail");

            var fromName = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromName");

            var msg = await EmailContentHelpers.PrepareEmailContentForClient(reportRequestObject, fileStream, fromEmail,
                fromName);

            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                reportRequestObject.Progress.Add(ExportingProgress.SendEmailToClient);
            }
            else
            {
                reportRequestObject.Progress.Add(ExportingProgress.FailSendingEmailToClient);
                reportRequestObject.ErrorMessage = response.Body.ToString();
                reportRequestObject.Status = ExportingStatus.Failure;
            }
        }
        catch (Exception ex)
        {
            reportRequestObject.Progress.Add(ExportingProgress.FailSendingEmailToClient);
            reportRequestObject.ErrorMessage = ex.Message;
            reportRequestObject.Status = ExportingStatus.Failure;
        }


        return reportRequestObject;
    }
}