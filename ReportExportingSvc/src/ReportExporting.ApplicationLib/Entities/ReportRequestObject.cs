using ReportExporting.Core;

namespace ReportExporting.ApplicationLib.Entities;

public class ReportRequestObject : ReportRequest
{
    public ReportRequestObject()
    {
        Progress = new List<ExportingProgress>();
    }

    public Guid Id { get; set; }
    public string? FileName { get; set; }
    public ExportingStatus Status { get; set; }
    public List<ExportingProgress> Progress { get; set; }

    public string GetFullProgress()
    {
        return string.Join(", ", Progress.ToArray());
    }
}