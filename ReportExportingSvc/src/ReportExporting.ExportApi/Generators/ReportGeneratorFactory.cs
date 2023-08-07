using ReportExporting.ApplicationLib.Entities;
using ReportExporting.Core;
using ReportExporting.ExportApi.Models;

namespace ReportExporting.ExportApi.Generators;

public class ReportGeneratorFactory : IReportGeneratorService
{
    private readonly PdfReportGenerator _pdfReportGenerator;
    private readonly WordReportGenerator _wordReportGenerator;

    public ReportGeneratorFactory(PdfReportGenerator pdfReportGenerator, WordReportGenerator wordReportGenerator)
    {
        _pdfReportGenerator = pdfReportGenerator;
        _wordReportGenerator = wordReportGenerator;
    }

    public async Task<Stream?> GenerateReport(ReportRequestObject request)
    {
        var exportConfiguration = ExportConfigurationFactory.GetConfiguration(request);
        var exportObject = ExportObjectFactory.CreateExportObject(request);
        return request.Format switch
        {
            ReportFormat.Pdf => await _pdfReportGenerator.GenerateReportAsync(exportObject, exportConfiguration),
            ReportFormat.Word => await _wordReportGenerator.GenerateReportAsync(exportObject, exportConfiguration),
            _ => await _pdfReportGenerator.GenerateReportAsync(exportObject, exportConfiguration)
        };
    }
}