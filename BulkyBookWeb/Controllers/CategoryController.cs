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
                dbx.Categories.Add(cat);
                dbx.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat);
        }
    }
}
