using Microsoft.Extensions.Configuration;
using SautinSoft;

namespace ReportExporting.ExportApi.Helpers.Core;

public class WordEngineWrapper(IConfiguration configuration) : IWordEngineWrapper
{
    public void SetLicense()
    {
        PdfFocus.SetLicense(configuration["PdfToWordLicense"]);
    }

    public PdfFocus GetRenderer()
    {
        return new PdfFocus();
    }
}