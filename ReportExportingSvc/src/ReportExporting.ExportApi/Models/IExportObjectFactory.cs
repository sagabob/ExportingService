using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ExportApi.Models.Core;

namespace ReportExporting.ExportApi.Models;

public interface IExportObjectFactory
{
    ExportObject CreateExportObject(ReportRequestObject reportRequest);
}