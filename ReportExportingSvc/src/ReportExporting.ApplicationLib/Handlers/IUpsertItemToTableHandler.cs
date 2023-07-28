using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Handlers;

public interface IUpsertItemToTableHandler
{
    Task<ReportRequestObject> Handle(ReportRequestObject request);
}