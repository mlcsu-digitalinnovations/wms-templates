using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Services
{
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
            var container = new BlobContainerClient(_BlobConnectionString, _BlobContainer);
            var blob = container.GetBlobClient(name);
            if (await blob.ExistsAsync())
            {
                return await blob.DownloadAsync();                
            }
            return null;
        }

       
    }
}
