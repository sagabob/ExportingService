using ReportExporting.ExportApi.Helpers;
using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Generators.Core;

public class WordReportGenerator : IWordReportGenerator
{
    private readonly IPdfReportGenerator _pdfReportGenerator;
    private readonly IWordEngineWrapper _wordEngineWrapper;

    public WordReportGenerator(IPdfReportGenerator pdfReportGenerator, IWordEngineWrapper wordEngineWrapper)
    {
        _pdfReportGenerator = pdfReportGenerator;
        _wordEngineWrapper = wordEngineWrapper;
    }


    public async Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config)
    {
        var pdfStream = await _pdfReportGenerator.GenerateReportAsync(exportObject, config);

        var wordEngine = _wordEngineWrapper.GetRenderer();

        wordEngine.OpenPdf(pdfStream);

        var output = wordEngine.ToWord();

        return new MemoryStream(output);
    }
}