using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
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
        private IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork un, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = un;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int? page = 1)
        {
            return View();
        }


        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                product = new(),
                categoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                coverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                // Create
                return View(productVM);
            }
            else
            {
                productVM.product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM prodVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wRoot = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wRoot, @"images\products\");
                    var extension = Path.GetExtension(file.FileName);

                    if(prodVM.product.ImgUrl != null)
                    {
                        var oldImagePath = Path.Combine(wRoot, prodVM.product.ImgUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads+fileName+extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    prodVM.product.ImgUrl = @"\images\products\"+fileName+extension;
                }

                if(prodVM.product.Id == 0)
                {
                    _unitOfWork.Product.Add(prodVM.product);
                    _unitOfWork.Save();
                    TempData["successmsg"] = prodVM.product.Title + " Added Successfully!";
                }
                else
                {
                    _unitOfWork.Product.Update(prodVM.product);
                    _unitOfWork.Save();
                    TempData["successmsg"] = prodVM.product.Title + " Updated Successfully!";
                }
                return RedirectToAction("Index");
            }

            return View(prodVM);
        }

        [HttpGet]
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

            if (prodFromDb.ImgUrl != null)
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, prodFromDb.ImgUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Product.Delete(prodFromDb);
            _unitOfWork.Save();
            TempData["successmsg"] = prodFromDb.Title + " Removed Successfully!";
            return RedirectToAction("Index");
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = productList });
        }

        #endregion

    }
}
