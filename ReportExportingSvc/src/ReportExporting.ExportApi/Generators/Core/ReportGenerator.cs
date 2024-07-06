using ConvertApiDotNet;
using Microsoft.Extensions.Configuration;
using ReportExporting.Core;
using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Generators.Core;

public class ReportGenerator(IConfiguration configuration) : IReportGenerator
{
    private readonly ConvertApi _convertApi = new(Convert.ToString(configuration["ConvertApiKey"])!);

    public async Task<Stream?> GenerateReportAsync(ExportObject exportObject, ExportConfiguration config)
    {
        var fileUrls = exportObject.Urls!
            .Select(urlItem => new ConvertApiBaseParam(urlItem.Title, new Uri(urlItem.Url))).ToList();
        var convertResult =
            await _convertApi.ConvertAsync("html", exportObject.Format == ReportFormat.Pdf ? "pdf" : "docx",
                fileUrls.ToArray());

        var convertedFile = convertResult.Files[0];

        return await convertedFile.FileStreamAsync();
    }
}