using ReportExporting.ExportApi.Helpers;
using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Generators.Core;

public class PdfReportGenerator(IPdfEngineWrapper pdfEngineWrapper) : IPdfReportGenerator
{
    public async Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config)
    {
        var renderer = pdfEngineWrapper.GetRenderer();
        var pdfDocuments = pdfEngineWrapper.CreateList();
        ;

        if (config.ShowCoverPage)
        {
            var cover = await RenderCoverPage(renderer, config, exportObject);
            renderer.RenderingOptions.FirstPageNumber = 2;
            pdfDocuments.Add(cover);
        }


        foreach (var urlItem in exportObject.Urls!)
        {
            var pdf = await renderer.RenderUrlAsPdfAsync(urlItem.Url);
            pdfDocuments.Add(pdf);
        }

        var merged = pdfEngineWrapper.MergeDocuments(pdfDocuments).Stream;

        return merged;
    }


    public async Task<PdfDocument> RenderCoverPage(ChromePdfRenderer renderer, ExportConfiguration config,
        ExportObject exportObject)
    {
        var cover = await renderer.RenderHtmlAsPdfAsync($"<h1>{exportObject.Product.ToString()}</h1>");
        return cover;
    }
}