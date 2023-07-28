using ReportExporting.ApplicationLib.Entities;
using ReportExporting.Core;
using ReportExporting.ExportApi.Generators;

namespace ReportExporting.ExportApi.Handlers
{
    public class ExportRequestHandler: IExportRequestHandler
    {
        private readonly IReportGeneratorService _reportGeneratorService;

        public ExportRequestHandler (IReportGeneratorService reportGeneratorService )
        {
            _reportGeneratorService = reportGeneratorService;
        }

        public async Task<Stream?> ProcessExportRequest(ReportRequestObject request)
        {
            Stream? exportedFileStream = null;
            try
            {
                exportedFileStream = await _reportGeneratorService.GenerateReport(request);

                request.FileName =
                    $"{request.Product}-{request.Id}.{(request.Format == ReportFormat.Pdf ? "pdf" : "docx")}";

                request.Progress.Add(request.Format == ReportFormat.Pdf
                    ? ExportingProgress.ExportedPdf
                    : ExportingProgress.ExportedWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                request.Progress.Add(request.Format == ReportFormat.Pdf
                    ? ExportingProgress.FailExportingPdf
                    : ExportingProgress.FailExportingWord);
                request.Status = ExportingStatus.Failure;
            }
           

            return exportedFileStream;

        }
    }
}
