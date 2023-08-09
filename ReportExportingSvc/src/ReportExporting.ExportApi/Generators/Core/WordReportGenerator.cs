using ReportExporting.ExportApi.Helpers;
using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Generators.Core;

public class WordReportGenerator : IReportGenerator
{
    private readonly PdfReportGenerator _pdfReportGenerator;
    private readonly IWordEngineWrapper _wordEngineWrapper;

    public WordReportGenerator(PdfReportGenerator pdfReportGenerator, IWordEngineWrapper wordEngineWrapper)
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