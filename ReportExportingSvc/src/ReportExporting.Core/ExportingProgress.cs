namespace ReportExporting.Core;

public enum ExportingProgress
{
    Submitting,
    PutOnStore,
    FailToPutOnStore,
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