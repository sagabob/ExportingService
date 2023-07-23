using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Requests;
using ReportExporting.PlaceOrderApi.Services;

namespace ReportExporting.PlaceOrderApi.Handlers
{
    public class UploadRequestHandler : IRequestHandler<AddItemToTableRequest, ReportRequestEntity>
    {
        private readonly ITableStorageService _tableTableStorageService;
        public UploadRequestHandler(ITableStorageService tableTableStorageService)
        {
            _tableTableStorageService = tableTableStorageService;
        }
        public async Task<ReportRequestEntity> Handle(AddItemToTableRequest request, CancellationToken cancellationToken)
        {
            await _tableTableStorageService.AddEntityAsync(request.PayLoad);
            return request.PayLoad;
        }
    }
}
