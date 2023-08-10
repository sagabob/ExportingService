using ReportExporting.ApplicationLib.Entities;
using ReportExporting.NotificationApi.Helpers;
using ReportExporting.NotificationApi.Helpers.Core;
using SendGrid;

namespace ReportExporting.NotificationApi.Services.Core;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailContentHelpers _emailContentHelpers;
    private readonly ISendGridClient _sendGridClient;

    public EmailService(
        ISendGridClient sendGridClient, IEmailContentHelpers emailContentHelpers,
        IConfiguration configuration)
    {
        _sendGridClient = sendGridClient;
        _emailContentHelpers = emailContentHelpers;
        _configuration = configuration;
    }

    public AdminInfo AdminInfo =>
        new()
        {
            FromMail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromEmail")!,
            FromName = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromName")!,
            AdminEmail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("AdminEmail")!
        };

    public async Task<ReportRequestObject> SendingEmailToAdminAsync(ReportRequestObject reportRequestObject)
    {
        try
        {
            //email to admin
            var stream = _emailContentHelpers.WrapReportRequestObjectToStream(reportRequestObject);

            var msg = await _emailContentHelpers.PrepareEmailContentForAdmin(reportRequestObject, stream,
                AdminInfo.FromMail,
                AdminInfo.FromMail, AdminInfo.AdminEmail);

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
            var msg = await _emailContentHelpers.PrepareEmailContentForClient(reportRequestObject, fileStream,
                AdminInfo.FromMail,
                AdminInfo.FromName
            );

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

    public async Task<Response> SendingEmailWithErrorToAdminAsync(ReportRequestErrorObject reportRequestErrorObject)
    {
        var msg = _emailContentHelpers.CreateMessageForAdminFromErrorMessage(reportRequestErrorObject,
            AdminInfo.FromMail,
            AdminInfo.FromName, AdminInfo.AdminEmail);

        return await _sendGridClient.SendEmailAsync(msg);
    }
}