using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Web;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork un)
        {
            _unitOfWork = un;
        }

        public IActionResult Index(int? page = 1)
        {
            List<Category> catList = _unitOfWork.Category.GetAll().ToList();
            //List<Category> catList = new List<Category>();    // for testing

            int pageSize = 5;
            int totalPages = catList.Count() / pageSize;
            totalPages += catList.Count() % pageSize == 0 ? 0 : 1;

            ViewData["PageSize"] = pageSize;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = catList.Count() > 0 ? totalPages : 0;

            int ind = (page.GetValueOrDefault() - 1) * pageSize;
            int len = catList.Count() >= ind + pageSize ? pageSize : catList.Count() - ind;

            return View(catList.GetRange(ind, len));
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
            if (cat.Name == cat.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Display order cannot match the name");
            }

            if (ModelState.IsValid)
            {
                cat.CreatedDateTime = DateTime.Now;
                _unitOfWork.Category.Add(cat);
                _unitOfWork.Save();
                TempData["successmsg"] = cat.Name + " Category Created Successfully!";
                return RedirectToAction("Index");
            }

            return View(cat);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var catFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (catFromDb == null)
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
                _unitOfWork.Category.Update(cat);
                _unitOfWork.Save();
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

            var catFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
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

            var catFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (catFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Delete(catFromDb);
            _unitOfWork.Save();
            TempData["successmsg"] = catFromDb.Name + " Category Removed Successfully!";
            return RedirectToAction("Index");
        }
    }
}
