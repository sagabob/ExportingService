﻿namespace ReportExporting.Core;

public class ReportRequest
{
    public string FileName { get; set; }
    public ReportUrl[] Urls { get; set; }
    public string EmailAddress { get; set; }
    public Guid? Guid { get; set; }
    public ReportFormat Format { get; set; }
    public string Title { get; set; }
    public ReportProduct Product { get; set; }
    public ExportingAttempt Attempt { get; set; }


}