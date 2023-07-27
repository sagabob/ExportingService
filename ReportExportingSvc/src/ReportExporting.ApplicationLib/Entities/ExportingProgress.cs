namespace ReportExporting.ApplicationLib.Entities;

public enum ExportingProgress
{
    Submitting,
    UpsertToStore,
    FailToUpsertToStore,
    PlaceOnQueue,
    FailToPlaceOnQueue,
    OnDeadLetterQueue,
    ExportingPdf,
    FailExportingPdf,
    ExportedPdf,
    ExportingWord,
    FailExportingWord,
    ExportedWord,
    Complete
}