using MediatR;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Requests;

namespace ReportExporting.PlaceOrderApi.Handlers
{
    public class UploadRequestHandler : IRequestHandler<AddItemToTableRequest, ReportRequest>
    {
        public Task<ReportRequest> Handle(AddItemToTableRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
