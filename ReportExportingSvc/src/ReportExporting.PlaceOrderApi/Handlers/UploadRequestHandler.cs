using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Requests;
using ReportExporting.PlaceOrderApi.Services;

namespace ReportExporting.PlaceOrderApi.Handlers
{
    public class UploadRequestHandler : IRequestHandler<AddItemToTableRequest, ReportRequestEntity>
    {
        public async Task<ReportRequestEntity> Handle(AddItemToTableRequest request, CancellationToken cancellationToken)
        {
            return request.PayLoad;
        }
    }
}
