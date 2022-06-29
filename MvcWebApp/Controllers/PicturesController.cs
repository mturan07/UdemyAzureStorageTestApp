using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using AzureStorageLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcWebApp.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MvcWebApp.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class PicturesController : Controller
    {
        public string UserId { get; set; } = "12345";
        public string City { get; set; } = "Antalya";

        private readonly INoSqlStorage<UserPicture> _noSqlStorage;
        private readonly IBlobStorage _blobStorage;
        public PicturesController(INoSqlStorage<UserPicture> noSqlStorage, IBlobStorage blobStorage)
        {
            _noSqlStorage = noSqlStorage;
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {
            List<FileBlob> fileBlobs = new();

            var user = await _noSqlStorage.Get(UserId, City);

            if(user != null)
            {
                if (user.Paths.Count > 0)
                {
                    user.Paths.ForEach(x =>
                    {
                        fileBlobs.Add(new FileBlob { Name = x, Url = $"{_blobStorage.BlobURL}/{x}" });
                    });
                }
            }

            ViewBag.fileBlobs = fileBlobs;
            ViewBag.UserId = UserId;
            ViewBag.City = City;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IEnumerable<IFormFile> pictures)
        {
            List<string> picturesList = new();
            foreach (var item in pictures)
            {
                var newPictureName = await FileUploadHelper.UploadFileAsync(_blobStorage, item);

                //var newPictureName = $"{Guid.NewGuid()}{Path.GetExtension(item.FileName)}";
                //await _blobStorage.UploadAsync(item.OpenReadStream(), newPictureName, EContainerName.pictures);
                picturesList.Add(newPictureName);
            }
            var userPicture = await _noSqlStorage.Get(UserId, City);
            if(userPicture != null)
            {
                picturesList.AddRange(userPicture.Paths);
                userPicture.Paths = picturesList;
            }
            else
            {
                userPicture = new UserPicture
                {
                    RowKey = UserId,
                    PartitionKey = City,
                    Paths = picturesList
                };
            }
            await _noSqlStorage.Add(userPicture);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ShowWatermark()
        {
            List<FileBlob> fileBlobs = new List<FileBlob>();
            UserPicture userPicture = await _noSqlStorage.Get(UserId, City);

            userPicture.WatermarkPaths.ForEach(x =>
            {
                fileBlobs.Add(new FileBlob { Name = x, Url = $"{_blobStorage.BlobURL}/{EContainerName.watermarkpictures}/{x}" });
            });

            ViewBag.fileBlobs = fileBlobs;

            return View();
        }

        public async Task<IActionResult> AddWatermark(PictureWatermarkQueue pictureWatermarkQueue)
        {
            var jsonString = JsonConvert.SerializeObject(pictureWatermarkQueue);
            string jsonStringBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            AzQueue azQueue = new("watermarkqueue");
            await azQueue.SendMessageAsync(jsonStringBase64);

            return Ok();
        }
    }
}
