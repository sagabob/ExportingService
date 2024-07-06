using Microsoft.Extensions.Configuration;

namespace ReportExporting.ExportApi.Helpers.Core;

public class PdfEngineWrapper(IConfiguration configuration) : IPdfEngineWrapper
{
    public void SetLicense()
    {
        License.LicenseKey = configuration["IronPdfLicense"];
    }

    public ChromePdfRenderer GetRenderer()
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.WaitFor.RenderDelay(5000);
        renderer.RenderingOptions.Timeout = 60;

        return renderer;
    }

    public List<PdfDocument> CreateList()
    {
        return new List<PdfDocument>();
    }

    public PdfDocument MergeDocuments(List<PdfDocument> documents)
    {
        return PdfDocument.Merge(documents);
    }
}