using ReportExporting.ApplicationLib.Entities;
using ReportExporting.Core;
using ReportExporting.ExportApi.Generators;

namespace ReportExporting.ExportApi.Handlers.Core;

public class ExportRequestHandler : IExportRequestHandler
{
    private readonly IReportGeneratorFactory _reportGeneratorFactory;

    public ExportRequestHandler(IReportGeneratorFactory reportGeneratorFactory)
    {
        _reportGeneratorFactory = reportGeneratorFactory;
    }

    public async Task<Stream?> ProcessExportRequest(ReportRequestObject request)
    {
        Stream? exportedFileStream = null;
        try
        {
            exportedFileStream = await _reportGeneratorFactory.GenerateReport(request);

            request.FileName =
                $"{request.Product}-{request.Id}.{(request.Format == ReportFormat.Pdf ? "pdf" : "docx")}";

            request.Progress.Add(request.Format == ReportFormat.Pdf
                ? ExportingProgress.ExportedPdf
                : ExportingProgress.ExportedWord);
        }
        catch (Exception)
        {
            request.Progress.Add(request.Format == ReportFormat.Pdf
                ? ExportingProgress.FailExportingPdf
                : ExportingProgress.FailExportingWord);
            request.Status = ExportingStatus.Failure;
            request.FileName = null;
        }


        return exportedFileStream;
    }
}