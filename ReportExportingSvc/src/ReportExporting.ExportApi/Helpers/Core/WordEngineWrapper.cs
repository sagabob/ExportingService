using Microsoft.Extensions.Configuration;
using SautinSoft;

namespace ReportExporting.ExportApi.Helpers.Core;

public class WordEngineWrapper: IWordEngineWrapper
{
    private readonly IConfiguration _configuration;

    public WordEngineWrapper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SetLicense()
    {
        PdfFocus.SetLicense(_configuration["PdfToWordLicense"]);
    }

    public PdfFocus GetRenderer()
    {
        return new PdfFocus();
    }
}