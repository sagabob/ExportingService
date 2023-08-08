using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Generators;

public interface IReportGenerator
{
    Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config);
}