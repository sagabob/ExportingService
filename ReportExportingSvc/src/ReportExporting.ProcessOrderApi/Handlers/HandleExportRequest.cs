using ReportExporting.ApplicationLib.Entities;
using ReportExporting.Core;
using ReportExporting.ExportApi.Generators;

namespace ReportExporting.ProcessOrderApi.Handlers
{
    public class HandleExportRequest: IHandleExportRequest
    {
        private readonly IReportGeneratorService _reportGeneratorService;

        public HandleExportRequest (IReportGeneratorService reportGeneratorService )
        {
            _reportGeneratorService = reportGeneratorService;
        }

        public void ProcessExportRequest(ReportRequestObject request)
        {
            var output = _reportGeneratorService.GenerateReport(request);

            request.FileName =
                $"{request.Product}-{request.Id}.{(request.Format == ReportFormat.Pdf? "pdf": "docx")}";


        }
    }
}
