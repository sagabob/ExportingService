using Microsoft.Extensions.Configuration;
using ReportExporting.ExportApi.Models.Core;
using SautinSoft;

namespace ReportExporting.ExportApi.Generators.Core;

public class WordReportGenerator : IReportGenerator
{
    private readonly PdfReportGenerator _pdfReportGenerator;
    private readonly PdfFocus _pdfToWordConverter;

    public WordReportGenerator(PdfReportGenerator pdfReportGenerator, IConfiguration configuration)
    {
        _pdfReportGenerator = pdfReportGenerator;

        PdfFocus.SetLicense(configuration["PdfToWordLicense"]);
        _pdfToWordConverter = new PdfFocus();
    }
    

    public async Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config)
    {
        var pdfStream = await _pdfReportGenerator.GenerateReportAsync(exportObject, config);

        _pdfToWordConverter.OpenPdf(pdfStream);

        var output = _pdfToWordConverter.ToWord();

        return new MemoryStream(output);
    }
}