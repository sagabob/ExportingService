namespace ReportExporting.ExportApi.Helpers;

public interface IPdfEngineWrapper
{
    ChromePdfRenderer GetRenderer();

    List<PdfDocument> CreateList();

    PdfDocument MergeDocuments(List<PdfDocument> documents);

    void SetLicense();
}