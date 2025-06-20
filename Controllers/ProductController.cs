using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VirtualStore.Data;
using VirtualStore.Models;
using static VirtualStore.Data.AppDbContext;

namespace VirtualStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int storeId, int spaceId)
        {
            ViewBag.StoreId = storeId;
            ViewBag.SpaceId = spaceId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(int storeId, int spaceId, Product product)
        {
            var space = _context.Spaces
                .Include(sp => sp.Products)
                .FirstOrDefault(sp => sp.Id == spaceId);

            if (space != null && ModelState.IsValid)
            {
                product.Id = _context.Products.Any() ? _context.Products.Max(p => p.Id) + 1 : 1;

                product.SpaceId = spaceId;

                _context.Products.Add(product);   
                space.Products.Add(product);      

                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        public IActionResult Edit(int storeId, int spaceId, int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.StoreId = storeId;
            ViewBag.SpaceId = spaceId;
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int storeId, int spaceId, Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            var existing = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Count = product.Count;
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        public IActionResult Delete(int storeId, int spaceId, int id)
        {
            var space = _context.Spaces
                .Include(sp => sp.Products)
                .FirstOrDefault(sp => sp.Id == spaceId);

            var product = space?.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                space.Products.Remove(product);
                _context.Products.Remove(product); 
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        [HttpGet]
        public IActionResult Move(int storeId, int fromSpaceId, int productId)
        {
            var store = _context.Stores
                .Include(s => s.Spaces)
                .ThenInclude(sp => sp.Products)
                .FirstOrDefault(s => s.Id == storeId);

            if (store == null) return NotFound();

            var spaces = store.Spaces.Where(sp => sp.Id != fromSpaceId).ToList();
            if (!spaces.Any())
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
            if (fromSpaceId == toSpaceId)
            {
                TempData["PopupMessage"] = "Cannot move product to the same space.";
                return RedirectToAction("Details", "Store", new { id = storeId });
            }

            var fromSpace = _context.Spaces
                .Include(sp => sp.Products)
                .FirstOrDefault(sp => sp.Id == fromSpaceId);

            var toSpace = _context.Spaces
                .Include(sp => sp.Products)
                .FirstOrDefault(sp => sp.Id == toSpaceId);

            var product = fromSpace?.Products.FirstOrDefault(p => p.Id == productId);

            if (fromSpace != null && toSpace != null && product != null)
            {
                fromSpace.Products.Remove(product);
                toSpace.Products.Add(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Store", new { id = storeId });
        }
    }
}
