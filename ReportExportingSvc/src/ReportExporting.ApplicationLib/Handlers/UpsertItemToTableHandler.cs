using MediatR;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Messages;
using ReportExporting.ApplicationLib.Services;

namespace ReportExporting.ApplicationLib.Handlers;

public class UpsertItemToTableHandler : IRequestHandler<UpsertItemToTableRequest, ReportRequestObject>
{
    private readonly ITableStorageService _tableTableStorageService;

    public UpsertItemToTableHandler(ITableStorageService tableTableStorageService)
    {
        _tableTableStorageService = tableTableStorageService;
    }

    public async Task<ReportRequestObject> Handle(UpsertItemToTableRequest request, CancellationToken cancellationToken)
    {
        request.PayLoad.Progress.Add(ExportingProgress.UpsertToStore);
        try
        {
            var response =
                await _tableTableStorageService.AddEntityAsync(
                    ReportRequestTableEntityFactory.CreateTableEntity(request.PayLoad));

            if (response.Status != 204)
            {
                request.PayLoad.Progress.Add(ExportingProgress.FailToUpsertToStore);
                request.PayLoad.Status = ExportingStatus.Failure;
            }
        }
        catch (Exception)
        {
            request.PayLoad.Progress.Add(ExportingProgress.FailToUpsertToStore);
            request.PayLoad.Status = ExportingStatus.Failure;
        }


        return request.PayLoad;
    }
}