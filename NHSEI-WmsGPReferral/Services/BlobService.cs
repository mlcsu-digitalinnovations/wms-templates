using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Services;

public interface IBlobService
{
  Task<BlobDownloadInfo> Download(string name);
}
public class BlobService : IBlobService
{
  private readonly string _BlobConnectionString = string.Empty;
  private readonly string _BlobContainer = string.Empty;
  public BlobService(IConfiguration configuration)
  {
    _BlobConnectionString = configuration["WmsTemplate:AzureBlobConnectionString"];
    _BlobContainer = "downloads";
  }

  public async Task<BlobDownloadInfo> Download(string name)
  {
    BlobContainerClient container = new(_BlobConnectionString, _BlobContainer);
    BlobClient blob = container.GetBlobClient(name);
    if (await blob.ExistsAsync())
    {
      return await blob.DownloadAsync();
    }

    return null;
  }
}
