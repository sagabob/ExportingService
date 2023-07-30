using Azure;
using Azure.Storage.Blobs.Models;

namespace ReportExporting.ApplicationLib.Services;

public interface IBlobStorageService
{
    Task<Response<BlobContentInfo>> UploadExportFileAync(Stream fileStream, string? blobName);

    Task<Response> DownloadExportFileAync(string? fileName, Stream fileStream);
}