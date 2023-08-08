using ReportExporting.Core;
using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Models;

public interface IExportConfigurationFactory
{
    ExportConfiguration GetConfiguration(ReportRequest reportRequest);
}