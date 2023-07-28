namespace ReportExporting.ApplicationLib.Entities;

public enum ExportingProgress
{
    Submitting,
    UpsertToStore,
    FailToUpsertToStore,
    PlaceOnQueue,
    FailToPlaceOnQueue,
    ItemReceivedFromQueue,
    OnDeadLetterQueue,
    FailExportingPdf,
    ExportedPdf,
    FailExportingWord,
    ExportedWord,
    UploadFileToBlob,
    FailUploadingFileToBlob,
    Complete
}