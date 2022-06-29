﻿using Azure.Storage.Blobs;
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

        public string BlobURL => "http://127.0.0.1:10000/devstoreaccount1/pictures";

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

            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.logs.ToString());

            await containerClient.CreateIfNotExistsAsync();

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

        public List<string> GetNames(EContainerName eContainerName)
        {
            List<string> blobNames = new();

            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            var blobs = containerClient.GetBlobs();
            blobs.ToList().ForEach(
                x => blobNames.Add(x.Name));
            return blobNames;
        }

        public async Task SetLogAsync(string text, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.logs.ToString());

            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);

            await appendBlobClient.CreateIfNotExistsAsync();

            using (MemoryStream ms = new())
            {
                using (StreamWriter sw = new(ms))
                {
                    sw.Write($"{DateTime.Now}:{text}\n");

                    sw.Flush();
                    ms.Position = 0;

                    await appendBlobClient.AppendBlockAsync(ms);
                }
            }
        }

        public async Task UploadAsync(Stream fileStream, string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());
            await containerClient.CreateIfNotExistsAsync();

            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
        }
    }
}
