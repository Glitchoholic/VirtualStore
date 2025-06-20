using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VirtualStore.Data;
using VirtualStore.Models;
using static VirtualStore.Data.AppDbContext;

namespace VirtualStore.Controllers
{
    public class SpaceController : Controller
    {
        private readonly AppDbContext _context;

        public SpaceController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Edit(int storeId, int spaceId)
        {
            var space = _context.Spaces
                .Include(sp => sp.Products)
                .FirstOrDefault(sp => sp.Id == spaceId && _context.Stores.Any(s => s.Id == storeId && s.Spaces.Contains(sp)));

            if (space == null) return NotFound();

            ViewBag.StoreId = storeId;
            return View(space);
        }

        [HttpPost]
        public IActionResult Edit(int storeId, Space space)
        {
            var existing = _context.Spaces.FirstOrDefault(sp => sp.Id == space.Id);
            if (existing != null)
            {
                existing.Name = space.Name;
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        public IActionResult Delete(int storeId, int spaceId)
        {
            var store = _context.Stores
                .Include(s => s.Spaces)
                .ThenInclude(sp => sp.Products)
                .FirstOrDefault(s => s.Id == storeId);

            var space = store?.Spaces.FirstOrDefault(sp => sp.Id == spaceId);

            if (store == null || space == null || store.Spaces.Count <= 1)
            {
                TempData["PopupMessage"] = "You cannot delete the only space in the store.";
                return RedirectToAction("Details", "Store", new { id = storeId });
            }

            _context.Spaces.Remove(space);
            _context.SaveChanges();

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        [HttpGet]
        public IActionResult Split(int storeId, int spaceId)
        {
            ViewBag.StoreId = storeId;
            ViewBag.SpaceId = spaceId;
            return View();
        }

        [HttpPost]
        public IActionResult Split(int storeId, int spaceId, int count)
        {
            if (count < 2) return BadRequest("Split count must be at least 2.");

            var store = _context.Stores
                .Include(s => s.Spaces)
                .FirstOrDefault(s => s.Id == storeId);

            var original = store?.Spaces.FirstOrDefault(sp => sp.Id == spaceId);
            if (store == null || original == null) return NotFound();

            int maxId = _context.Spaces.Any() ? _context.Spaces.Max(sp => sp.Id) : 0;

            for (int i = 2; i <= count; i++)
            {
                var newSpace = new Space
                {
                    Id = ++maxId,
                    Name = $"{original.Name} {i}",
                    Products = new List<Product>()
                };

                store.Spaces.Add(newSpace);
                _context.Spaces.Add(newSpace);
            }

            _context.SaveChanges();

            return RedirectToAction("Details", "Store", new { id = storeId });
        }


        [HttpGet]
        public IActionResult Merge(int storeId, int spaceId1)
        {
            var store = _context.Stores
                .Include(s => s.Spaces)
                .ThenInclude(sp => sp.Products)
                .FirstOrDefault(s => s.Id == storeId);

            if (store == null) return RedirectToAction("Index");

            var spaceToKeep = store.Spaces.FirstOrDefault(sp => sp.Id == spaceId1);
            if (spaceToKeep == null) return RedirectToAction("Details", "Store", new { id = storeId });

            var otherSpaces = store.Spaces.Where(sp => sp.Id != spaceId1).ToList();
            if (!otherSpaces.Any())
            {
                TempData["PopupMessage"] = "No other space to merge.";
                return RedirectToAction("Details", "Store", new { id = storeId });
            }

            ViewBag.StoreId = storeId;
            ViewBag.SpaceId1 = spaceId1;
            ViewBag.Spaces = otherSpaces;
            ViewBag.Space1Name = spaceToKeep.Name;

            return View();
        }


        [HttpPost]
        public IActionResult Merge(int storeId, int spaceId1, int spaceId2)
        {
            if (spaceId1 == spaceId2)
            {
                TempData["PopupMessage"] = "Cannot merge a space with itself.";
                return RedirectToAction("Merge", new { storeId, spaceId1 });
            }

            var store = _context.Stores
                .Include(s => s.Spaces)
                .ThenInclude(sp => sp.Products)
                .FirstOrDefault(s => s.Id == storeId);

            var s1 = store?.Spaces.FirstOrDefault(s => s.Id == spaceId1);
            var s2 = store?.Spaces.FirstOrDefault(s => s.Id == spaceId2);

            if (store == null || s1 == null || s2 == null)
                return BadRequest("One or both spaces not found.");

            foreach (var product in s2.Products)
            {
                s1.Products.Add(product);
            }

            store.Spaces.Remove(s2);
            _context.Spaces.Remove(s2);

            _context.SaveChanges();

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

    }
}
