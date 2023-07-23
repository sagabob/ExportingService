namespace ReportExporting.Core;

public class ExportingAttempt
{
    public List<ExportingProgress> Attempts { get; set; }

    public ExportingProgress CurrentProgress { get; set; }
}