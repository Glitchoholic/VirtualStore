using Microsoft.AspNetCore.Mvc;
using VirtualStore.Models;
using System.Linq;

namespace VirtualStore.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index(string name, string address, string isMain, string isInvoiceDirect, string search)
        {
            var stores = FakeDatabase.Stores.AsQueryable();

            // Apply filters only if user clicked "Search"
            bool isSearchTriggered = !string.IsNullOrEmpty(search);

            bool mainValue = isMain?.ToLower() == "true";
            bool invoiceValue = isInvoiceDirect?.ToLower() == "true";

            if (isSearchTriggered)
            {
                if (!string.IsNullOrWhiteSpace(name))
                    stores = stores.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(address))
                    stores = stores.Where(s => s.Address.Contains(address, StringComparison.OrdinalIgnoreCase));

                // Apply true/false filters — unchecked means false
                stores = stores.Where(s => s.IsMain == mainValue);
                stores = stores.Where(s => s.IsInvoiceDirect == invoiceValue);
            }

            ViewBag.SearchName = name;
            ViewBag.SearchAddress = address;
            ViewBag.SearchIsMain = mainValue;
            ViewBag.SearchIsInvoiceDirect = invoiceValue;

            return View(stores.ToList());
        }



        public IActionResult Details(int id)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == id);
            if (store == null)
                return NotFound();

            return View(store);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Store store)
        {
          
                store.Id = FakeDatabase.Stores.Max(s => s.Id) + 1;
                store.Spaces = new List<Space>
                {
                    new Space
                    {
                        Id = FakeDatabase.Stores.SelectMany(s => s.Spaces).Max(sp => sp.Id) + 1,
                        Name = "Default Space",
                        Products = new List<Product>()
                    }
                };

                FakeDatabase.Stores.Add(store);
                return RedirectToAction(nameof(Index));
            

          //  return View(store);
        }

        public IActionResult Edit(int id)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return NotFound();
            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Store updatedStore)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return NotFound();

            store.Name = updatedStore.Name;
            store.Address = updatedStore.Address;
            store.IsMain = updatedStore.IsMain;
            store.IsInvoiceDirect = updatedStore.IsInvoiceDirect;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return NotFound();

            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return NotFound();

            FakeDatabase.Stores.Remove(store);
            return RedirectToAction(nameof(Index));
        }
    }
}
