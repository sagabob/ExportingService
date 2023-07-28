namespace ReportExporting.Core;

public class ReportRequest
{
    public ReportUrl[] Urls { get; set; }
    public string EmailAddress { get; set; }
    public ReportFormat Format { get; set; }
    public string Title { get; set; }
    public ReportProduct Product { get; set; }
}