using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Services;

namespace ReportExporting.ApplicationLib.Handlers.Core;

public class UploadItemToBlobHandler : IUploadItemToBlobHandler
{
    private readonly IBlobStorageService _blobStorageService;

    public UploadItemToBlobHandler(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    public async Task<ReportRequestObject> Handle(Stream fileStream, ReportRequestObject request)
    {
        if (request.Status == ExportingStatus.Failure)
            return request;

        try
        {
            request.Progress.Add(ExportingProgress.UploadFileToBlob);
            var response = await _blobStorageService.UploadExportFileAync(fileStream, request.FileName);

            if (!response.HasValue)
            {
                request.Progress.Add(ExportingProgress.FailUploadingFileToBlob);
                request.Status = ExportingStatus.Failure;
            }
        }
        catch (Exception)
        {
            request.Progress.Add(ExportingProgress.FailUploadingFileToBlob);
            request.Status = ExportingStatus.Failure;
        }

        return request;
    }
}