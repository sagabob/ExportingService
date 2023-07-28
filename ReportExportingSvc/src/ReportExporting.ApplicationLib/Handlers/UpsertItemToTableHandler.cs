using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Services;

namespace ReportExporting.ApplicationLib.Handlers;

public class UpsertItemToTableHandler : IUpsertItemToTableHandler
{
    private readonly ITableStorageService _tableTableStorageService;

    public UpsertItemToTableHandler(ITableStorageService tableTableStorageService)
    {
        _tableTableStorageService = tableTableStorageService;
    }

    public async Task<ReportRequestObject> Handle(ReportRequestObject request)
    {
        request.Progress.Add(ExportingProgress.UpsertToStore);
        try
        {
            var tableEntity = ReportRequestTableEntityFactory.CreateTableEntity(request);
            var response =
                await _tableTableStorageService.AddEntityAsync(tableEntity);

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