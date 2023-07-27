using ReportExporting.Core;
using ReportExporting.ExportApi.Generators;

namespace ReportExporting.ProcessOrderApi.Handlers
{
    public class HandleExportRequest
    {
        private readonly IReportGeneratorService _reportGeneratorService;

        public HandleExportRequest (IReportGeneratorService reportGeneratorService )
        {
            _reportGeneratorService = reportGeneratorService;
        }

        public void ProcessExportRequest(ReportRequest request)
        {
            var output = _reportGeneratorService.GenerateReport(request);

            var fileName =
                $"{request.Product}-{request.Guid}.{(request.Format == ReportFormat.Pdf? "pdf": "docx")}";


        }
    }
}
