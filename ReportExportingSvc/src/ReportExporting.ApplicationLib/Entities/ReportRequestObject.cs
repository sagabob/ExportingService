using ReportExporting.Core;

namespace ReportExporting.ApplicationLib.Entities;

public class ReportRequestObject : ReportRequest
{
    public Guid Id { get; init; }
    public string? FileName { get; set; }
    public ExportingStatus Status { get; set; }
    public List<ExportingProgress> Progress { get; } = new();

    public string? ErrorMessage { get; set; }

    public string GetFullProgress()
    {
        return string.Join(", ", Progress.ToArray());
    }
}