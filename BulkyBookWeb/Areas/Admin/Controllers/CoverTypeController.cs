using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Web;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetailes.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork un)
        {
            _unitOfWork = un;
        }

        public IActionResult Index(int? page = 1)
        {
            List<CoverType> covList = _unitOfWork.CoverType.GetAll().ToList();
            //List<CoverType> covList = new List<CoverType>();    // for testing

            int pageSize = 5;
            int totalPages = covList.Count() / pageSize;
            totalPages += covList.Count() % pageSize == 0 ? 0 : 1;

            ViewData["PageSize"] = pageSize;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = covList.Count() > 0 ? totalPages : 0;

            int ind = (page.GetValueOrDefault() - 1) * pageSize;
            int len = covList.Count() >= ind + pageSize ? pageSize : covList.Count() - ind;

            return View(covList.GetRange(ind, len));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType cov)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(cov);
                _unitOfWork.Save();
                TempData["successmsg"] = cov.Name + " CoverType Created Successfully!";
                return RedirectToAction("Index");
            }

            return View(cov);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var covFromDb = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (covFromDb == null)
            {
                return NotFound();
            }

            return View(covFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType cov)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(cov);
                _unitOfWork.Save();
                TempData["successmsg"] = cov.Name + " CoverType Updated Successfully!";
                return RedirectToAction("Index");
            }

            return View(cov);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var covFromDb = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (covFromDb == null)
            {
                return NotFound();
            }

            return View(covFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var covFromDb = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (covFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.CoverType.Delete(covFromDb);
            _unitOfWork.Save();
            TempData["successmsg"] = covFromDb.Name + " CoverType Removed Successfully!";
            return RedirectToAction("Index");
        }
    }
}
