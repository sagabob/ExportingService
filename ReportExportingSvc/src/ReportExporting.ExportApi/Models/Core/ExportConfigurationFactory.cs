using ReportExporting.Core;

namespace ReportExporting.ExportApi.Models.Core;

public class ExportConfigurationFactory: IExportConfigurationFactory
{
    public ExportConfiguration GetConfiguration(ReportRequest reportRequest)
    {
        var config = new ExportConfiguration
        {
            ShowPageNumber = true,
            ShowCoverPage = true
        };
        if (reportRequest.Product == ReportProduct.Economy)
            config.ShowPageNumber = false;

        return config;
    }
}