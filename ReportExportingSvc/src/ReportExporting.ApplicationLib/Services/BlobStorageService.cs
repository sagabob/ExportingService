using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace ReportExporting.ApplicationLib.Services;

public class BlobStorageService : IBlobStorageService
{
    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        BlobClient = blobServiceClient.GetBlobContainerClient(configuration["ContainerName"]);
    }

    public BlobContainerClient BlobClient { get; }

    public async Task<Response<BlobContentInfo>> UploadExportFileAync(Stream fileStream, string? blobName)
    {
        var response = await BlobClient.UploadBlobAsync(blobName, fileStream);

        return response;
    }

    public async Task<Response> DownloadExportFileAync(string? fileName,  Stream fileStream)
    {
        var blob = BlobClient.GetBlobClient(fileName);

        var response = await blob.DownloadToAsync(fileStream);

        return response;
    }
}