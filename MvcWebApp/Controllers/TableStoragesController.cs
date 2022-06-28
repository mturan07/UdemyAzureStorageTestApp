using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebApp.Controllers
{
    public class TableStoragesController : Controller
    {
        private readonly INoSqlStorage<Product> _noSqlStorage;
        public TableStoragesController(INoSqlStorage<Product> noSqlStorage)
        {
            _noSqlStorage = noSqlStorage;
        }

        public IActionResult Index()
        {
            ViewBag.products = _noSqlStorage.GetAll().ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Urunler";

            await _noSqlStorage.Add(product);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            ViewBag.IsUpdate = true;
            await _noSqlStorage.Update(product);
            return RedirectToAction("Index");
        }
    }
}
