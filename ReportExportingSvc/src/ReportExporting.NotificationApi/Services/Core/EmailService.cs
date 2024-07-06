using ReportExporting.ApplicationLib.Entities;
using ReportExporting.NotificationApi.Helpers;
using ReportExporting.NotificationApi.Helpers.Core;
using SendGrid;

namespace ReportExporting.NotificationApi.Services.Core;

public class EmailService(
    ISendGridClient sendGridClient,
    IEmailContentHelpers emailContentHelpers,
    IConfiguration configuration)
    : IEmailService
{
    public AdminInfo AdminInfo =>
        new()
        {
            FromMail = Convert.ToString(configuration["SendGridEmailSettings:FromEmail"])!,
            FromName = Convert.ToString(configuration["SendGridEmailSettings:FromName"])!,
            AdminEmail = Convert.ToString(configuration["SendGridEmailSettings:AdminEmail"])!
        };

    public async Task<ReportRequestObject> SendingEmailToAdminAsync(ReportRequestObject reportRequestObject)
    {
        try
        {
            //email to admin
            var stream = emailContentHelpers.WrapReportRequestObjectToStream(reportRequestObject);

            var msg = await emailContentHelpers.PrepareEmailContentForAdmin(reportRequestObject, stream,
                AdminInfo.FromMail,
                AdminInfo.FromMail, AdminInfo.AdminEmail);

            var response = await sendGridClient.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                reportRequestObject.Progress.Add(ExportingProgress.SendEmailToAdmin);
            }
            else
            {
                reportRequestObject.Progress.Add(ExportingProgress.FailSendingEmailToAdmin);
                reportRequestObject.ErrorMessage = ExportingProgress.FailSendingEmailToAdmin.ToString();
            }
        }
        catch (Exception ex)
        {
            reportRequestObject.Progress.Add(ExportingProgress.FailSendingEmailToAdmin);
            reportRequestObject.ErrorMessage = ex.Message;
        }
        //email to admin

        return reportRequestObject;
    }

    public async Task<ReportRequestObject> SendingEmailToClientAsync(ReportRequestObject reportRequestObject,
        Stream fileStream)
    {
        try
        {
            var msg = await emailContentHelpers.PrepareEmailContentForClient(reportRequestObject, fileStream,
                AdminInfo.FromMail,
                AdminInfo.FromName
            );

            var response = await sendGridClient.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                reportRequestObject.Progress.Add(ExportingProgress.SendEmailToClient);
            }
            else
            {
                reportRequestObject.Progress.Add(ExportingProgress.FailSendingEmailToClient);
                reportRequestObject.ErrorMessage = ExportingProgress.FailSendingEmailToClient.ToString();
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
        var msg = emailContentHelpers.CreateMessageForAdminFromErrorMessage(reportRequestErrorObject,
            AdminInfo.FromMail,
            AdminInfo.FromName, AdminInfo.AdminEmail);

        return await sendGridClient.SendEmailAsync(msg);
    }
}