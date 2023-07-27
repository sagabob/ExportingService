using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace ReportExporting.ApplicationLib.Services;

public class BlobStorageService: IBlobStorageService
{
    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        BlobClient = blobServiceClient.GetBlobContainerClient(configuration["ContainerName"]);
        BlobClient.CreateIfNotExists();
    }

    public async Task<Response<BlobContentInfo>> UploadExportFileAync(Stream fileStream, string? blobName)
    {
        var response =  await BlobClient.UploadBlobAsync(blobName, fileStream);

        return response;
    }
    public BlobContainerClient BlobClient { get; }
}