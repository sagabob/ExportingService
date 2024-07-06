using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ExportApi.Models;

namespace ReportExporting.ExportApi.Generators.Core;

public class ReportGeneratorFactory(
    IReportGenerator reportGenerator,
    IExportConfigurationFactory exportConfigurationFactory,
    IExportObjectFactory exportObjectFactory)
    : IReportGeneratorFactory
{
    public async Task<Stream?> GenerateReport(ReportRequestObject request)
    {
        var exportConfiguration = exportConfigurationFactory.GetConfiguration(request);
        var exportObject = exportObjectFactory.CreateExportObject(request);


        return await reportGenerator.GenerateReportAsync(exportObject, exportConfiguration);
    }
}