using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace AzureStorageLibrary.Services
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobStorage()
        {
            _blobServiceClient = new BlobServiceClient(ConnectionStrings.AzureConnectionString);
        }

        public string BlobURL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task DeleteAsync(string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteAsync();
        }

        public async Task<Stream> DownloadAsync(string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            var blobClient = containerClient.GetBlobClient(fileName);

            var fileInfo = await blobClient.DownloadContentAsync();
            return fileInfo.Value.Content.ToStream();
        }

        public async Task<List<string>> GetLogAsync(string fileName)
        {
            List<string> logs = new();

            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.Logs.ToString());
            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);

            await appendBlobClient.CreateIfNotExistsAsync();

            var info = await appendBlobClient.DownloadContentAsync();

            using (StreamReader sr = new(info.Value.Content.ToStream()))
            {
                string line = String.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    logs.Add(line);
                }
            }

            return logs;
        }

        public Task<List<string>> GetNamesAsync(EContainerName eContainerName)
        {
            throw new NotImplementedException();
        }

        public async Task SetLogAsync(string text, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.Logs.ToString());
            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);

            await appendBlobClient.CreateIfNotExistsAsync();


        }

        public async Task UploadAsync(Stream fileStream, string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            await containerClient.CreateIfNotExistsAsync();

            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileName);
        }
    }
}
