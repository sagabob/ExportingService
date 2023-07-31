using ReportExporting.Core;

namespace ReportExporting.ExportApi.Models;

public class ExportObject
{
    public string Id { get; set; }

    public ReportUrl[]? Urls { get; set; }

    public ReportProduct Product { get; set; }

    public ReportFormat Format { get; set; }
}