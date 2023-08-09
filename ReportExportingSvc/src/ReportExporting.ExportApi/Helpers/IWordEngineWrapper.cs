using SautinSoft;

namespace ReportExporting.ExportApi.Helpers;

public interface IWordEngineWrapper
{
    void SetLicense();

    PdfFocus GetRenderer();
}