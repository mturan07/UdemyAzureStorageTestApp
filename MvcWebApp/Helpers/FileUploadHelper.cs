using AzureStorageLibrary;

namespace MvcWebApp.Helpers
{
    public static class FileUploadHelper
    {
        public static async Task<string> UploadFileAsync(IBlobStorage blobStorage, IFormFile formFile)
        {
            var workDir = Directory.GetCurrentDirectory();

            string filePath = Path.Combine(workDir, formFile.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                formFile.CopyTo(fileStream);
            }

            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);

            string destPath = Path.Combine(workDir, newFileName);

            File.Move(filePath, destPath);

            await blobStorage.UploadAsync(formFile.OpenReadStream(), newFileName, EContainerName.pictures);

            File.Delete(destPath);

            return newFileName;
        }
    }
}
