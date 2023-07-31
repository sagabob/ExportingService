namespace ReportExporting.Core;

public class ReportRequest
{
    public ReportUrl[] Urls { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public ReportFormat Format { get; set; }

    public string Title { get; set; } = null!;

    public ReportProduct Product { get; set; }
}