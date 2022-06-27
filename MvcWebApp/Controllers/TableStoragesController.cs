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

        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Kalemler";

            await _noSqlStorage.Add(product);

            return RedirectToAction("Index");
        }
    }
}
