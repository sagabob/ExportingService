using ReportExporting.ExportApi.Helpers;
using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Generators.Core;

public class WordReportGenerator(IPdfReportGenerator pdfReportGenerator, IWordEngineWrapper wordEngineWrapper)
    : IWordReportGenerator
{
    public async Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config)
    {
        var pdfStream = await pdfReportGenerator.GenerateReportAsync(exportObject, config);

        var wordEngine = wordEngineWrapper.GetRenderer();

        wordEngine.OpenPdf(pdfStream);

        var output = wordEngine.ToWord();

        return new MemoryStream(output);
    }
}