using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Services;

namespace ReportExporting.ApplicationLib.Handlers.Core;

public class UpsertItemToTableHandler : IUpsertItemToTableHandler
{
    private readonly IReportRequestTableEntityFactory _reportRequestTableEntityFactory;
    private readonly ITableStorageService _tableTableStorageService;

    public UpsertItemToTableHandler(ITableStorageService tableTableStorageService,
        IReportRequestTableEntityFactory reportRequestTableEntityFactory)
    {
        _tableTableStorageService = tableTableStorageService;
        _reportRequestTableEntityFactory = reportRequestTableEntityFactory;
    }

    public async Task<ReportRequestObject> Handle(ReportRequestObject request)
    {
        request.Progress.Add(ExportingProgress.UpsertToStore);
        try
        {
            var tableEntity = _reportRequestTableEntityFactory.CreateTableEntity(request);
            var response =
                await _tableTableStorageService.UpsertEntityAsync(tableEntity);

            if (response.Status != 204)
            {
                request.Progress.Add(ExportingProgress.FailToUpsertToStore);
                request.Status = ExportingStatus.Failure;
            }
        }
        catch (Exception)
        {
            request.Progress.Add(ExportingProgress.FailToUpsertToStore);
            request.Status = ExportingStatus.Failure;
        }


        return request;
    }
}