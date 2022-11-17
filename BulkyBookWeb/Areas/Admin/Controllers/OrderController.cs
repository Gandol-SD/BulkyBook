using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? page = 1, string? status = "all")
        {
            //List<OrderHeader> orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            // Role Filter
            List<OrderHeader> orderListA;
            if (User.IsInRole(StaticDetailes.Role_Admin) || User.IsInRole(StaticDetailes.Role_Employee))
            {
                orderListA = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderListA = _unitOfWork.OrderHeader.GetAllWhere(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser").ToList();
            }

            List<OrderHeader> orderList = new List<OrderHeader>();

            if (status == "all")
            {
                orderList = orderListA;
            }
            else
            {
                foreach (var order in orderListA)
                {
                    if (status == order.OrderStatus) orderList.Add(order);
                }
            }


            int pageSize = 5;
            int totalPages = orderList.Count() / pageSize;
            totalPages += orderList.Count() % pageSize == 0 ? 0 : 1;

            ViewData["PageSize"] = pageSize;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = orderList.Count() > 0 ? totalPages : 0;

            int ind = (page.GetValueOrDefault() - 1) * pageSize;
            int len = orderList.Count() >= ind + pageSize ? pageSize : orderList.Count() - ind;

            return View(orderList.GetRange(ind, len));
        }

        public IActionResult Details(int orderId)
        {
            orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAllWhere(u => u.OrderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult PayNow(int orderId)
        {   
            orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAllWhere(u => u.OrderId == orderId, includeProperties: "Product")
            };

            // >>>>>>>>>>>> Stripe
            var domain = "https://localhost:44322/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?id={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/Details?orderId={orderVM.OrderHeader.Id}",
            };

            foreach (var item in orderVM.OrderDetails)
            {
                var sLineItemOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), //2000,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },

                    },
                    Quantity = item.Count,
                };

                options.LineItems.Add(sLineItemOptions);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            orderVM.OrderHeader.SessionId = session.Id;
            orderVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            // <<<<<<<<<<<< Stripe
        }

        public IActionResult PaymentConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);

            if (orderHeader.PaymentStatus == StaticDetailes.PaymentStatusDelayed)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                // Check stripe's status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, orderHeader.SessionId, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, orderHeader.OrderStatus, StaticDetailes.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            return View(id);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetailes.Role_Admin + "," + StaticDetailes.Role_Employee)]
        public IActionResult UpdateOrderDetail(int orderId)
        {
            var orderHeaderFromDB = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);
            orderHeaderFromDB.Name = orderVM.OrderHeader.Name;
            orderHeaderFromDB.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDB.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDB.City = orderVM.OrderHeader.City;
            orderHeaderFromDB.State = orderVM.OrderHeader.State;
            orderHeaderFromDB.PostalCode = orderVM.OrderHeader.PostalCode;
            if(orderVM.OrderHeader.Carrier != null)
            {
                orderHeaderFromDB.Carrier = orderVM.OrderHeader.Carrier;
            }
            if(orderVM.OrderHeader.TrackingNumber != null)
            {
                orderHeaderFromDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDB);
            _unitOfWork.Save();
            TempData["successmsg"] = "Order Details updated successfully!";

            return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDB.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetailes.Role_Admin + "," + StaticDetailes.Role_Employee)]
        public IActionResult StartProcessing(int orderId)
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderId, StaticDetailes.StatusInProcessing);
            _unitOfWork.Save();

            TempData["successmsg"] = "Order is in progress";
            return RedirectToAction("Details", "Order", new { orderId = orderId });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetailes.Role_Admin + "," + StaticDetailes.Role_Employee)]
        public IActionResult ShipOrder(int orderId)
        {
            OrderHeader orderFromDB = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);

            orderFromDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderFromDB.Carrier = orderVM.OrderHeader.Carrier;
            orderFromDB.OrderStatus = StaticDetailes.StatusShipped;
            orderFromDB.ShippingDate = DateTime.Now;

            if(orderFromDB.PaymentStatus == StaticDetailes.PaymentStatusDelayed)
            {
                orderFromDB.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeader.Update(orderFromDB);
            _unitOfWork.Save();

            TempData["successmsg"] = "Order shipped successfully!";
            return RedirectToAction("Details", "Order", new { orderId = orderId });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetailes.Role_Admin + "," + StaticDetailes.Role_Employee)]
        public IActionResult CancelOrder(int orderId)
        {
            OrderHeader orderFromDB = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId);

            if(orderFromDB.PaymentStatus == StaticDetailes.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDB.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                orderFromDB.OrderStatus = StaticDetailes.StatusCancelled;
                orderFromDB.PaymentStatus = StaticDetailes.StatusRefunded;
            }
            else
            {
                orderFromDB.OrderStatus = StaticDetailes.StatusCancelled;
                orderFromDB.PaymentStatus = StaticDetailes.StatusCancelled;
            }

            _unitOfWork.OrderHeader.Update(orderFromDB);
            _unitOfWork.Save();

            TempData["successmsg"] = "Order Cancelled successfully!";
            return RedirectToAction("Details", "Order", new { orderId = orderId });
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
