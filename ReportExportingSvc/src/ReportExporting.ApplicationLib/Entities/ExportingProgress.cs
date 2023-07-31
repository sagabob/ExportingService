namespace ReportExporting.ApplicationLib.Entities;

public enum ExportingProgress
{
    Submitting,
    UpsertToStore,
    FailToUpsertToStore,
    PlaceOrderOnQueue,
    FailToPlaceOrderOnQueue,
    OrderReceivedFromQueue,
    OrderOnDeadLetterQueue,
    FailExportingPdf,
    ExportedPdf,
    FailExportingWord,
    ExportedWord,
    UploadFileToBlob,
    FailUploadingFileToBlob,
    DownloadBlobToStream,
    FailDownloadingBlobToStream,
    SendOrderToEmailQueue,
    FailSendingOrderToEmailQueue,
    OrderReceivedFromEmailQueue,
    SendEmailToClient,
    SendEmailToAdmin,
    FailSendingEmailToClient,
    FailSendingEmailToAdmin,
    Complete
}