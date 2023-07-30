using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.NotificationApi.Services;

namespace ReportExporting.NotificationApi.Handlers.Core;

public class SendEmailHandler : ISendEmailHandler
{
    private readonly IDownloadItemFromBlobHandler _downloadItemFromBlobHandler;
    private readonly IEmailService _emailService;
    private readonly IUpsertItemToTableHandler _upsertItemToTableHandler;

    public SendEmailHandler(IEmailService emailService, IDownloadItemFromBlobHandler downloadItemFromBlobHandler,
        IUpsertItemToTableHandler upsertItemToTableHandler)
    {
        _emailService = emailService;
        _downloadItemFromBlobHandler = downloadItemFromBlobHandler;
        _upsertItemToTableHandler = upsertItemToTableHandler;
    }


    public async Task<ReportRequestObject> HandleSendingEmailToClient(ReportRequestObject reportRequestObject)
    {
        var fileStream = new MemoryStream();
        var blobResult = await
            _downloadItemFromBlobHandler.Handle(fileStream, reportRequestObject);

        var updatedResult = await _upsertItemToTableHandler.Handle(blobResult);

        if (blobResult.Status == ExportingStatus.Failure)
        {
            updatedResult = await _emailService.SendingEmailToAdminAsync(updatedResult);

            updatedResult = await _upsertItemToTableHandler.Handle(updatedResult);
        }
        else
        {
            updatedResult = await _emailService.SendingEmailToClientAsync(updatedResult, fileStream);

            if (updatedResult.Status != ExportingStatus.Failure)
            {
                updatedResult.Status = ExportingStatus.Success;

                //final update to the table
                updatedResult = await _upsertItemToTableHandler.Handle(updatedResult);
            }
            else
            {
                updatedResult = await _emailService.SendingEmailToAdminAsync(updatedResult);

                updatedResult = await _upsertItemToTableHandler.Handle(updatedResult);
            }
        }

        return updatedResult;
    }

    public async Task<ReportRequestObject> HandleSendingEmailToAdmin(ReportRequestObject reportRequestObject)
    {
        return await _emailService.SendingEmailToAdminAsync(reportRequestObject);
    }
}