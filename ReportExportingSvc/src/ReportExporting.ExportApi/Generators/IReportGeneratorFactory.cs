using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ExportApi.Generators;

public interface IReportGeneratorFactory
{
    Task<Stream?> GenerateReport(ReportRequestObject request);
}