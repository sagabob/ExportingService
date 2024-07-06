using ReportExporting.ApplicationLib.Entities;
using ReportExporting.Core;
using ReportExporting.ExportApi.Models;

namespace ReportExporting.ExportApi.Generators.Core;

public class ReportGeneratorFactory(
    IPdfReportGenerator pdfReportGenerator,
    IWordReportGenerator wordReportGenerator,
    IExportConfigurationFactory exportConfigurationFactory,
    IExportObjectFactory exportObjectFactory)
    : IReportGeneratorFactory
{
    public async Task<Stream?> GenerateReport(ReportRequestObject request)
    {
        var exportConfiguration = exportConfigurationFactory.GetConfiguration(request);
        var exportObject = exportObjectFactory.CreateExportObject(request);

        if (request.Format == ReportFormat.Word)
            return await wordReportGenerator.GenerateReportAsync(exportObject, exportConfiguration);

        return await pdfReportGenerator.GenerateReportAsync(exportObject, exportConfiguration);
    }
}