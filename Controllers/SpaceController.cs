using Microsoft.AspNetCore.Mvc;
using VirtualStore.Models;

namespace VirtualStore.Controllers
{
    public class SpaceController : Controller
    {
        public IActionResult Edit(int storeId, int spaceId)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId);
            var space = store?.Spaces.FirstOrDefault(sp => sp.Id == spaceId);
            if (space == null) return NotFound();

            ViewBag.StoreId = storeId;
            return View(space);
        }

        [HttpPost]
        public IActionResult Edit(int storeId, Space space)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId);
            var existing = store?.Spaces.FirstOrDefault(sp => sp.Id == space.Id);
            if (existing != null)
                existing.Name = space.Name;

            return RedirectToAction("Details", "Store", new { id = storeId });
        }

        public IActionResult Delete(int storeId, int spaceId)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId);
            var space = store?.Spaces.FirstOrDefault(sp => sp.Id == spaceId);

            if (store == null || space == null || store.Spaces.Count <= 1)
            {
                TempData["PopupMessage"] = "You cannot delete the only space in the store.";
                return RedirectToAction("Details", "Store", new { id = storeId });
            }

            store.Spaces.Remove(space);
            return RedirectToAction("Details", "Store", new { id = storeId });
        }


        // GET: Show split form
        [HttpGet]
        public IActionResult Split(int storeId, int spaceId)
        {
            ViewBag.StoreId = storeId;
            ViewBag.SpaceId = spaceId;
            return View();
        }

        // POST: Perform split
        [HttpPost]
        public IActionResult Split(int storeId, int spaceId, int count)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId);
            var original = store?.Spaces.FirstOrDefault(sp => sp.Id == spaceId);
            if (original == null || count < 2) return BadRequest();

            int maxId = FakeDatabase.Stores.SelectMany(s => s.Spaces).Max(sp => sp.Id);

            var newSpaces = Enumerable.Range(1, count)
                .Select(i => new Space
                {
                    Id = ++maxId,
                    Name = $"{original.Name} {i}",
                    Products = i == 1 ? original.Products : new List<Product>() 
                })
                .ToList();

            store.Spaces.Remove(original);
            store.Spaces.AddRange(newSpaces);

            return RedirectToAction("Details", "Store", new { id = storeId });
        }


        // GET: Show merge form
        [HttpGet]
        public IActionResult Merge(int storeId, int spaceId1)
        {
            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId);
            var spaces = store?.Spaces
                .Where(sp => sp.Id != spaceId1) // Exclude current space
                .ToList();

            if (store == null || spaces == null || spaces.Count == 0)
            {
                TempData["PopupMessage"] = "You only have one space in the store.";
                return RedirectToAction("Details", "Store", new { id = storeId });
            }

            ViewBag.StoreId = storeId;
            ViewBag.SpaceId1 = spaceId1;
            ViewBag.Spaces = store.Spaces;
            return View();
        }


        // POST: Perform merge
        [HttpPost]
        public IActionResult Merge(int storeId, int spaceId1, int spaceId2)
        {
            if (spaceId1 == spaceId2)
            {
                ModelState.AddModelError("", "Cannot merge a space with itself.");
                return RedirectToAction("Merge", new { storeId });
            }

            var store = FakeDatabase.Stores.FirstOrDefault(s => s.Id == storeId);
            var s1 = store?.Spaces.FirstOrDefault(s => s.Id == spaceId1);
            var s2 = store?.Spaces.FirstOrDefault(s => s.Id == spaceId2);

            if (store == null || s1 == null || s2 == null || spaceId1 == spaceId2)
                return BadRequest();

            var newSpace = new Space
            {
                Id = FakeDatabase.Stores.SelectMany(s => s.Spaces).Max(sp => sp.Id) + 1,
                Name = s1.Name,
                Products = s1.Products.Concat(s2.Products).ToList()
            };

            store.Spaces.Remove(s1);
            store.Spaces.Remove(s2);
            store.Spaces.Add(newSpace);

            return RedirectToAction("Details", "Store", new { id = storeId });
        }
    }
}
