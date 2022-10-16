using BulkyBookWeb.Models;
using BulkyBookWeb.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext dbx;

        public CategoryController(AppDbContext db)
        {
            dbx = db;
        }

        public IActionResult Index()
        {
            List<Category> catList = dbx.Categories.ToList();

            return View(catList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category cat)
        {
            // Custom Validation Message
            if(cat.Name == cat.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Display order cannot match the name");
            }

            if (ModelState.IsValid)
            {
                cat.CreatedDateTime = DateTime.Now;
                dbx.Categories.Add(cat);
                dbx.SaveChanges();
                TempData["successmsg"] = cat.Name + " Category Created Successfully!";
                return RedirectToAction("Index");
            }

            return View(cat);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            var catFromDb = dbx.Categories.Find(id);
            if(catFromDb == null)
            {
                return NotFound();
            }

            return View(catFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category cat)
        {
            // Custom Validation Message
            if (cat.Name == cat.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Display order cannot match the name");
            }

            if (ModelState.IsValid)
            {
                dbx.Categories.Update(cat);
                dbx.SaveChanges();
                TempData["successmsg"] = cat.Name + " Category Updated Successfully!";
                return RedirectToAction("Index");
            }

            return View(cat);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var catFromDb = dbx.Categories.Find(id);
            if (catFromDb == null)
            {
                return NotFound();
            }

            return View(catFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var catFromDb = dbx.Categories.Find(id);
            if (catFromDb == null)
            {
                return NotFound();
            }

            dbx.Categories.Remove(catFromDb);
            dbx.SaveChanges();
            TempData["successmsg"] = catFromDb.Name + " Category Removed Successfully!";
            return RedirectToAction("Index");
        }
    }
}
