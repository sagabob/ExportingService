using ReportExporting.Core;

namespace ReportExporting.ExportApi.Models.Core;

public class ExportObject
{
    public required string Id { get; set; }

    public ReportUrl[]? Urls { get; init; }

    public ReportProduct Product { get; init; }

    public ReportFormat Format { get; set; }
}