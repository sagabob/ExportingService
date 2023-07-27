using Microsoft.Extensions.Configuration;
using ReportExporting.ExportApi.Models;

namespace ReportExporting.ExportApi.Generators;

public class PdfReportGenerator : IReportGenerator
{
    public PdfReportGenerator(IConfiguration configuration)
    {
        License.LicenseKey = configuration["IronPdfLicense"];
    }

    public async Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config)
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.WaitFor.RenderDelay(5000);
        renderer.RenderingOptions.Timeout = 60;

        var cover = await RenderCoverPage(renderer, config)!;

        var pdfDocuments = new List<PdfDocument> { cover };

        foreach (var urlItem in exportObject.Urls)
        {
            var pdf = await renderer.RenderUrlAsPdfAsync(urlItem.Url);
            pdfDocuments.Add(pdf);
        }

        var merged = PdfDocument.Merge(pdfDocuments).Stream;

        return merged;
    }


    public static Task<PdfDocument>? RenderCoverPage(ChromePdfRenderer renderer, ExportConfiguration config)
    {
        if (!config.ShowCoverPage) return null;
        var cover = renderer.RenderUrlAsPdfAsync("<h1> This is Cover Page</h1>");
        renderer.RenderingOptions.FirstPageNumber = 2;
        return cover;
    }
}