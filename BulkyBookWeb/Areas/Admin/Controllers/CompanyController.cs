using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Web;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork un)
        {
            _unitOfWork = un;
        }

        public IActionResult Index(int? page = 1)
        {
            List<Company> compList = _unitOfWork.Company.GetAll().ToList();
            //List<Company> compList = new List<Company>();    // for testing

            int pageSize = 5;
            int totalPages = compList.Count() / pageSize;
            totalPages += compList.Count() % pageSize == 0 ? 0 : 1;

            ViewData["PageSize"] = pageSize;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = compList.Count() > 0 ? totalPages : 0;

            int ind = (page.GetValueOrDefault() - 1) * pageSize;
            int len = compList.Count() >= ind + pageSize ? pageSize : compList.Count() - ind;

            return View(compList.GetRange(ind, len));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Company comp)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Add(comp);
                _unitOfWork.Save();
                TempData["successmsg"] = comp.Name + " Company Created Successfully!";
                return RedirectToAction("Index");
            }

            return View(comp);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var compFromDb = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (compFromDb == null)
            {
                return NotFound();
            }

            return View(compFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Company comp)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Update(comp);
                _unitOfWork.Save();
                TempData["successmsg"] = comp.Name + " Company Updated Successfully!";
                return RedirectToAction("Index");
            }

            return View(comp);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var compFromDb = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (compFromDb == null)
            {
                return NotFound();
            }

            return View(compFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var compFromDb = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (compFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Company.Delete(compFromDb);
            _unitOfWork.Save();
            TempData["successmsg"] = compFromDb.Name + " Company Removed Successfully!";
            return RedirectToAction("Index");
        }
    }
}
