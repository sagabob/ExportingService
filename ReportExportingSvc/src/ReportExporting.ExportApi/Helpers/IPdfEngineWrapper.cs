namespace ReportExporting.ExportApi.Helpers;

public interface IPdfEngineWrapper
{
    ChromePdfRenderer GetRender();

    List<PdfDocument> CreateList();

    PdfDocument MergeDocuments(List<PdfDocument> documents);
}