using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Printing;
using System.Web;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork un)
        {
            _unitOfWork = un;
        }

        public IActionResult Index(int? page = 1)
        {
            List<Product> prodList = _unitOfWork.Product.GetAll().ToList();
            //List<Product> prodList = new List<Product>();    // for testing

            int pageSize = 5;
            int totalPages = prodList.Count() / pageSize;
            totalPages += prodList.Count() % pageSize == 0 ? 0 : 1;

            ViewData["PageSize"] = pageSize;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = prodList.Count() > 0 ? totalPages : 0;

            int ind = (page.GetValueOrDefault() - 1) * pageSize;
            int len = prodList.Count() >= ind + pageSize ? pageSize : prodList.Count() - ind;

            return View(prodList.GetRange(ind, len));
        }

        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Product prod)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Add(prod);
        //        _unitOfWork.Save();
        //        TempData["successmsg"] = prod.Title + " Product Created Successfully!";
        //        return RedirectToAction("Index");
        //    }

        //    return View(prod);
        //}

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Product product = new Product();
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            IEnumerable<SelectListItem> coverTypeList = _unitOfWork.CoverType.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            if (id == null || id == 0)
            {
                // Create
                ViewBag.CategoryList = categoryList;
                ViewBag.CoverTypeList = coverTypeList;
                return View(product);
            }

            var prodFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (prodFromDb == null)
            {
                return NotFound();
            }

            return View(prodFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product prod)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(prod);
                _unitOfWork.Save();
                TempData["successmsg"] = prod.Title + " Product Updated Successfully!";
                return RedirectToAction("Index");
            }

            return View(prod);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var prodFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (prodFromDb == null)
            {
                return NotFound();
            }

            return View(prodFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var prodFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (prodFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Delete(prodFromDb);
            _unitOfWork.Save();
            TempData["successmsg"] = prodFromDb.Title + " Product Removed Successfully!";
            return RedirectToAction("Index");
        }
    }
}
