using Microsoft.AspNetCore.Mvc;
using VirtualStore.Models;

namespace VirtualStore.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Create(int storeId, int spaceId)
        {
            ViewBag.StoreId = storeId;
            ViewBag.SpaceId = spaceId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(int storeId, int spaceId, Product product)
        {
            var space = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId)
                        ?.Spaces.FirstOrDefault(sp => sp.Id == spaceId);

            product.Id = FakeDatabase.Stores.SelectMany(s => s.Spaces).SelectMany(p => p.Products).Max(p => p.Id) + 1;
            space?.Products.Add(product);

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        public IActionResult Edit(int storeId, int spaceId, int id)
        {
            var product = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId)
                          ?.Spaces.FirstOrDefault(sp => sp.Id == spaceId)
                          ?.Products.FirstOrDefault(p => p.Id == id);

            ViewBag.StoreId = storeId;
            ViewBag.SpaceId = spaceId;
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int storeId, int spaceId, Product product)
        {
            var prod = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId)
                       ?.Spaces.FirstOrDefault(sp => sp.Id == spaceId)
                       ?.Products.FirstOrDefault(p => p.Id == product.Id);

            if (prod != null)
            {
                prod.Name = product.Name;
                prod.Count = product.Count;
            }

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        public IActionResult Delete(int storeId, int spaceId, int id)
        {
            var space = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId)
                        ?.Spaces.FirstOrDefault(sp => sp.Id == spaceId);

            var product = space?.Products.FirstOrDefault(p => p.Id == id);
            if (product != null) space.Products.Remove(product);

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        [HttpGet]
        public IActionResult Move(int storeId, int fromSpaceId, int productId)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId);
            var spaces = store?.Spaces
                .Where(sp => sp.Id != fromSpaceId) // Exclude current space
                .ToList();

            if (store == null || spaces == null || spaces.Count == 0)
            {
                TempData["PopupMessage"] = "You only have one space in the store.";
                return RedirectToAction("Details", "Store", new { id = storeId });
            }

            ViewBag.StoreId = storeId;
            ViewBag.FromSpaceId = fromSpaceId;
            ViewBag.ProductId = productId;
            ViewBag.Spaces = spaces;

            return View(); 
        }


        [HttpPost]
        public IActionResult Move(int storeId, int productId, int fromSpaceId, int toSpaceId)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId);
            var from = store?.Spaces.FirstOrDefault(sp => sp.Id == fromSpaceId);
            var to = store?.Spaces.FirstOrDefault(sp => sp.Id == toSpaceId);
            var product = from?.Products.FirstOrDefault(p => p.Id == productId);

            if (product != null && to != null)
            {
                from.Products.Remove(product);
                to.Products.Add(product);
            }

            return RedirectToAction("Details", "Store", new { id = storeId });
        }
    }
}
