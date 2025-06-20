using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VirtualStore.Data;
using VirtualStore.Models;
using static VirtualStore.Data.AppDbContext;

namespace VirtualStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly AppDbContext _context;

        public StoreController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string name, string address, string isMain, string isInvoiceDirect, string search)
        {
            var stores = _context.Stores
                .Include(s => s.Spaces)
                .ThenInclude(sp => sp.Products)
                .AsQueryable();

            bool isSearchTriggered = !string.IsNullOrEmpty(search);
            bool mainValue = isMain?.ToLower() == "true";
            bool invoiceValue = isInvoiceDirect?.ToLower() == "true";

            if (isSearchTriggered)
            {
                if (!string.IsNullOrWhiteSpace(name))
                    stores = stores.Where(s => EF.Functions.Like(s.Name, $"%{name}%"));

                if (!string.IsNullOrWhiteSpace(address))
                    stores = stores.Where(s => EF.Functions.Like(s.Address, $"%{address}%"));

                stores = stores.Where(s => s.IsMain == mainValue);
                stores = stores.Where(s => s.IsInvoiceDirect == invoiceValue);
            }

            ViewBag.SearchName = name;
            ViewBag.SearchAddress = address;
            ViewBag.SearchIsMain = mainValue;
            ViewBag.SearchIsInvoiceDirect = invoiceValue;

  
            return View(stores);
        }

        public IActionResult Details(int id)
        {
            var store = _context.Stores
                .Include(s => s.Spaces)
                .ThenInclude(sp => sp.Products)
                .FirstOrDefault(s => s.Id == id);

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
            if (ModelState.IsValid)
            {
                int newStoreId = _context.Stores.Any() ? _context.Stores.Max(s => s.Id) + 1 : 1;
                int newSpaceId = _context.Spaces.Any() ? _context.Spaces.Max(s => s.Id) + 1 : 1;

                store.Id = newStoreId;
                store.Spaces = new List<Space>
                {
                    new Space
                    {
                        Id = newSpaceId,
                        Name = "Default Space",
                        Products = new List<Product>()
                    }
                };

                _context.Stores.Add(store);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(store);
        }

        public IActionResult Edit(int id)
        {
            var store = _context.Stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return NotFound();

            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Store updatedStore)
        {
            var store = _context.Stores
                .Include(s => s.Spaces)
                .ThenInclude(sp => sp.Products)
                .FirstOrDefault(s => s.Id == id);

            if (store == null) return NotFound();

            updatedStore.Spaces = store.Spaces;

            _context.Entry(store).CurrentValues.SetValues(updatedStore);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var store = _context.Stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return NotFound();

            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var store = _context.Stores
                .Include(s => s.Spaces)
                .ThenInclude(sp => sp.Products)
                .FirstOrDefault(s => s.Id == id);

            if (store == null) return NotFound();

            _context.Stores.Remove(store);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
