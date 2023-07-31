using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Helpers;

public interface IReportRequestTableEntityFactory
{
    ReportRequestTableEntity CreateTableEntity(ReportRequestObject request);
}