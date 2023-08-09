using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Generators;

public interface IPdfReportGenerator
{
    Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config);

    Task<PdfDocument> RenderCoverPage(ChromePdfRenderer renderer, ExportConfiguration config,
        ExportObject exportObject);
}