using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Generators;

public interface IWordReportGenerator
{
    Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config);
}