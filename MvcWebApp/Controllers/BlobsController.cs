using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using MvcWebApp.Helpers;

namespace MvcWebApp.Controllers
{
    public class BlobsController : Controller
    {
        private readonly IBlobStorage _blobStorage;

        public BlobsController(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {
            var names = _blobStorage.GetNames(EContainerName.pictures);
            string blobUrl = $"{_blobStorage.BlobURL}";
            ViewBag.blobs = names.Select(x => new FileBlob { Name = x, Url = $"{blobUrl}/{x}"}).ToList();

            ViewBag.logs = await _blobStorage.GetLogAsync("controller.txt");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            // log
            await _blobStorage.SetLogAsync("Upload started", "controller.txt");

            //var workDir = Directory.GetCurrentDirectory();            

            //string filePath = Path.Combine(workDir, picture.FileName);

            //using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            //{
            //    picture.CopyTo(fileStream);
            //}

            //var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);

            //string destPath = Path.Combine(workDir, newFileName);

            //System.IO.File.Move(filePath, destPath);

            //await _blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);

            //System.IO.File.Delete(destPath);

            await FileUploadHelper.UploadFileAsync(_blobStorage, picture);

            // log
            await _blobStorage.SetLogAsync("Upload finished", "controller.txt");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _blobStorage.DownloadAsync(fileName, EContainerName.pictures);
            return File(stream, "application/octet-stream", fileName);
        }

        public async Task<IActionResult> Delete(string fileName)
        {
            await _blobStorage.DeleteAsync(fileName, EContainerName.pictures);
            return RedirectToAction("Index");
        }
    }
}
