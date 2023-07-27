using ReportExporting.Core;

namespace ReportExporting.ExportApi.Generators;

public interface IReportGeneratorService
{
    Task<Stream> GenerateReport(ReportRequest request);
}