using ReportExporting.ApplicationLib.Entities;
using ReportExporting.Core;
using ReportExporting.ExportApi.Models;

namespace ReportExporting.ExportApi.Generators.Core;

public class ReportGeneratorFactory : IReportGeneratorFactory
{
    private readonly IExportConfigurationFactory _exportConfigurationFactory;
    private readonly IExportObjectFactory _exportObjectFactory;
    private readonly IPdfReportGenerator _pdfReportGenerator;
    private readonly IWordReportGenerator _wordReportGenerator;


    public ReportGeneratorFactory(IPdfReportGenerator pdfReportGenerator, IWordReportGenerator wordReportGenerator,
        IExportConfigurationFactory exportConfigurationFactory, IExportObjectFactory exportObjectFactory)
    {
        _pdfReportGenerator = pdfReportGenerator;
        _wordReportGenerator = wordReportGenerator;
        _exportConfigurationFactory = exportConfigurationFactory;
        _exportObjectFactory = exportObjectFactory;
    }

    public async Task<Stream?> GenerateReport(ReportRequestObject request)
    {
        var exportConfiguration = _exportConfigurationFactory.GetConfiguration(request);
        var exportObject = _exportObjectFactory.CreateExportObject(request);

        if (request.Format == ReportFormat.Word)
            return await _wordReportGenerator.GenerateReportAsync(exportObject, exportConfiguration);

        return await _pdfReportGenerator.GenerateReportAsync(exportObject, exportConfiguration);
    }
}