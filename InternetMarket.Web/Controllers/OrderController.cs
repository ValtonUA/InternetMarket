using InternetMarket.Domain.Core;
using InternetMarket.Infrastructure.Data;
using InternetMarket.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace InternetMarket.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        [Authorize(Roles = "Admin")]
        public ActionResult Orders(
            int page = 1,
            string sort = "OrderId",
            string sortdir = "asc",
            string search = "",
            string message = ""
            )
        {
            int pageSize = 10;
            int totalRecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pageSize) - pageSize;
            var data = _GetOrders(search, sort, sortdir, skip, pageSize, out totalRecord);
            ViewBag.TotalRows = totalRecord;
            ViewBag.Search = search;
            using (OrderContext db = new OrderContext())
            {
                ViewBag.Statuses = db.AllStatus.ToList();
            }

            var ordersVM = new List<OrderViewModel>();
            using (OrderContext db = new OrderContext())
            {
                foreach(Order o in data)
                {
                    var productNames = new List<string>();
                    var opList = db.OrderProducts.Where(i => i.OrderId == o.OrderId).ToList();

                    foreach (OrderProduct op in opList)
                        productNames.Add(op.ProductName);

                    ordersVM.Add(new OrderViewModel
                    {
                        OrderId = o.OrderId,
                        Status = o.Status,
                        Location = o.Location,
                        Date = o.Date,
                        UserLogin = o.UserLogin,
                        ProductNames = productNames
                    });
                }
            }

            ViewBag.Message = message;

            return View(ordersVM);
        }

        private List<Order> _GetOrders(
            string search,
            string sort,
            string sortdir,
            int skip,
            int pageSize,
            out int totalRecord)
        {
            using (OrderContext db = new OrderContext())
            {
                var data = db.Orders.Where(i =>
                    i.OrderId.ToString().Contains(search) ||
                    i.Status.Contains(search) ||
                    i.Location.Contains(search) ||
                    i.Date.ToString().Contains(search))
                    .OrderBy(sort + " " + sortdir);

                totalRecord = data.Count();

                if (pageSize > 0)
                {
                    data = data.Skip(skip).Take(pageSize);
                }

                return data.ToList();
            }
        }
        [Authorize(Roles = "Admin")]
        public JsonResult RemoveByStatus(string status)
        {
            bool success;
            string message;

            using (OrderContext db = new OrderContext())
            {
                try
                {
                    var orders = db.Orders.Where(i => i.Status.Equals(status));
                    db.Orders.RemoveRange(orders);

                    db.SaveChanges();

                    success = true;
                    message = "Successfully removed";
                }
                catch (Exception ex)
                {
                    success = false;
                    message = ex.Message;
                }
            }

            return Json(new { success, message } );
        }
        [Authorize(Roles = "Admin")]
        public JsonResult GetStatuses()
        {
            using (OrderContext db = new OrderContext())
            {
                var res = new List<string>();
                var statuses = db.AllStatus.ToList();

                foreach (Status s in statuses)
                {
                    res.Add(s.StatusName);
                }

                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void ChangeStatus(int orderId, string status)
        {
            using (OrderContext db = new OrderContext())
            {
                var order = db.Orders.Single(i => i.OrderId == orderId);
                order.Status = status;

                db.SaveChanges();
            }
        }
        [Authorize(Roles = "Customer")]
        public ActionResult Cart(
            int page = 1,
            string sort = "Category",
            string sortdir = "asc",
            string search = ""
            )
        {
            int pageSize = 10;
            int totalRecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pageSize) - pageSize;
            var data = _GetProductsFromCart(search, sort, sortdir, skip, pageSize, out totalRecord);
            ViewBag.TotalRows = totalRecord;
            ViewBag.Search = search;

            var productsVM = new List<ProductViewModel>();
            foreach (Product p in data)
                productsVM.Add(new ProductViewModel
                {
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Category = p.Category,
                    Rate = p.Rate,
                    Price = p.Price,
                    IsVoted = false
                });

            return View(productsVM);
        }

        private List<Product> _GetProductsFromCart(
            string search,
            string sort,
            string sortdir,
            int skip,
            int pageSize,
            out int totalRecord)
        {

            using (UserContext db = new UserContext())
            {
                var data = db.UserProducts.Where(i =>
                    string.Equals(i.UserLogin, User.Identity.Name));

                totalRecord = data.Count();

                using (ProductContext products = new ProductContext())
                {
                    List<Product> list = new List<Product>();
                    foreach (UserProduct up in data.ToList())
                    {
                        var product = products.Products.FirstOrDefault(i => i.ProductName == up.ProductName);
                        if (product != null)
                            list.Add(product);
                    }

                    list = list.OrderBy(sort + " " + sortdir).ToList();
                    if (pageSize > 0)
                    {
                        list = list.Skip(skip).Take(pageSize).ToList();
                    }

                    return list;
                }
            }
        }
        [Authorize(Roles = "Customer")]
        public JsonResult RemoveAll()
        {
            _RemoveFromCart();

            return Json(new { success = true, message = "Successfully removed"});
        }
        [Authorize(Roles = "Customer")]
        public JsonResult Remove(string productName)
        {
            _RemoveFromCart(productName);

            return Json(new { success = true, message = "\"" + productName + "\"" + " successfully removed" });
        }

        private void _RemoveFromCart(string productName = null)
        {
            if (productName == null)
            {
                using (UserContext db = new UserContext())
                {
                    foreach (UserProduct up in db.UserProducts)
                        db.UserProducts.Remove(up);

                    db.SaveChanges();
                }
            }
            else
            {
                using (UserContext db = new UserContext())
                {
                    var p = db.UserProducts.FirstOrDefault(i => string.Equals(i.ProductName, productName));
                    if (p != null)
                        db.UserProducts.Remove(p);

                    db.SaveChanges();
                }
            }
        }
        [Authorize(Roles = "Customer")]
        public ActionResult Checkout(string location)
        {
            using (OrderContext orderContext = new OrderContext())
            {
                // Creating a new order
                var order = orderContext.Orders.Add(new Order
                {
                    Status = "In transit",
                    Location = location,
                    Date = DateTime.Now,
                    UserLogin = User.Identity.Name
                });

                orderContext.SaveChanges();

                using (UserContext userContext = new UserContext())
                {
                    // Get products from a logged user cart
                    var cart = userContext.UserProducts.Where(i => string.Equals(i.UserLogin, User.Identity.Name));
                    // Attach the products to the order
                    foreach (UserProduct up in cart)
                        orderContext.OrderProducts.Add(new OrderProduct
                        {
                            OrderId = order.OrderId,
                            ProductName = up.ProductName
                        });
                }

                orderContext.SaveChanges();
            }
            // Erase the cart
            _RemoveFromCart();

            return RedirectToAction("Cart");
        }
    }
}