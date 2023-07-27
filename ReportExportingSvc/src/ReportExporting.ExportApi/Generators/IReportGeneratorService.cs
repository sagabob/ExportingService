using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ExportApi.Generators;

public interface IReportGeneratorService
{
    Task<Stream?> GenerateReport(ReportRequestObject request);
}