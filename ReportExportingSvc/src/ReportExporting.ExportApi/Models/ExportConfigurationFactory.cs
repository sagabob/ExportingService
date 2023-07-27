using ReportExporting.Core;

namespace ReportExporting.ExportApi.Models;

public class ExportConfigurationFactory
{
    public static ExportConfiguration GetConfiguration(ReportRequest reportRequest)
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