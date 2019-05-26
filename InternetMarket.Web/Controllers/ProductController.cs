using InternetMarket.Domain.Core;
using InternetMarket.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using InternetMarket.Web.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InternetMarket.Web.Controllers
{
    public class ProductController : Controller
    {
        // Products action
        public ActionResult Products(
            int page = 1,
            string sort = "Category",
            string sortdir = "asc",
            string search = "",
            string message  = ""
            )
        {
            int pageSize = 10; 
            int totalRecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pageSize) - pageSize;
            var data = _GetProducts(search, sort, sortdir, skip, pageSize, out totalRecord);
            ViewBag.TotalRows = totalRecord;
            ViewBag.Search = search;

            var productsVM = new List<ProductViewModel>();
            using (ProductContext db = new ProductContext())
            { 
                foreach (Product p in data)
                    productsVM.Add(new ProductViewModel
                    {
                        ProductName = p.ProductName,
                        Description = p.Description,
                        Category = p.Category,
                        Rate = p.Rate,
                        Price = p.Price,
                        IsVoted = db.ProductVotes.FirstOrDefault(i =>
                                    string.Equals(i.ProductName, p.ProductName) &&
                                    string.Equals(i.UserLogin, User.Identity.Name)) != null
                    });
            }

            ViewBag.Message = message;

            return View(productsVM);
        }

        private List<Product> _GetProducts(
            string search,
            string sort,
            string sortdir,
            int skip,
            int pageSize,
            out int totalRecord)
        {
            using (ProductContext db = new ProductContext())
            {
                var data = db.Products.Where(i =>
                    i.ProductName.Contains(search) ||
                    i.Description.Contains(search) ||
                    i.Category.Contains(search))
                    .OrderBy(sort + " " + sortdir);

                totalRecord = data.Count();

                if (pageSize > 0)
                {
                    data = data.Skip(skip).Take(pageSize);
                }

                return data.ToList();
            }
        }

        public ActionResult GetProductsVM()
        {
            using (ProductContext db = new ProductContext())
            {
                var res = new List<ProductViewModel>();
                foreach (Product p in db.Products)
                    res.Add(new ProductViewModel
                    {
                        ProductName = p.ProductName,
                        Description = p.Description,
                        Category = p.Category,
                        Rate = p.Rate,
                        Price = p.Price,
                        IsVoted = _IsVoted(p.ProductName)
                    });

                return Json(new { data = res }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool _IsVoted(string productName)
        {
            using (ProductContext db = new ProductContext()) {
                return db.ProductVotes.FirstOrDefault(i =>
                            string.Equals(productName, i.ProductName) &&
                            string.Equals(User.Identity.Name, i.UserLogin)) != null;
            }
        }

        // Products POST action
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public JsonResult AddToCart(string productName)
        {
            using (UserContext db = new UserContext())
            {
                db.UserProducts.Add(new UserProduct
                {
                    UserLogin = User.Identity.Name,
                    ProductName = productName
                });

                db.SaveChanges();
            }

            return Json(new { success = true, message = "Added to cart" });
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public JsonResult Vote(string rate, string productName)
        {
            string message;
            bool success;

            using (ProductContext db = new ProductContext())
            {
                // mark that the current user voted
                db.ProductVotes.Add(new ProductVote
                {
                    UserLogin = User.Identity.Name,
                    ProductName = productName
                });

                // Change rate
                var item = db.Products.First(i => i.ProductName == productName);
                item.NumberOfVotes++;
                item.Rate = (item.Rate + float.Parse(rate)) / item.NumberOfVotes;

                try
                {
                    db.SaveChanges();
                    success = true;
                    message = "Thanks for your vote!";
                }
                catch (Exception ex)
                {
                    success = false;
                    message = ex.Message;
                }
            }

            return Json(new { success, message });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddOrEdit(string productName = "")
        {
            using (ProductContext db = new ProductContext())
            {
                ViewBag.Categories = db.Categories.ToList();

                if (productName == "")
                {
                    return View(new CreateOrEditProductVM());
                }
                else
                {
                    var product = db.Products.FirstOrDefault(i => string.Equals(i.ProductName, productName));

                    return View(new CreateOrEditProductVM {
                        ProductName = product.ProductName,
                        Category = product.Category,
                        Description = product.Description,
                        Price = product.Price,
                        OldProductName = product.ProductName
                    });
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddOrEdit(CreateOrEditProductVM model)
        {
            string message = "";
            bool success;

            using (ProductContext db = new ProductContext())
            {
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Description = model.Description,
                    Category = model.Category,
                    Rate = 0,
                    NumberOfVotes = 0,
                    Price = model.Price
                };

                if (string.IsNullOrEmpty(model.OldProductName)) {
                    var item = db.Products.FirstOrDefault(
                        i => string.Equals(i.ProductName, model.ProductName));
                    if (item != null)
                    {
                        success = false;
                        message = "Such product is already exist.";
                    }
                    else
                    {
                        db.Products.Add(product);
                        success = true;
                        message = "Successfully added";
                    }
                }
                else
                {
                    if (!string.Equals(model.ProductName, model.OldProductName))
                    {
                        // verify whether a new product is unique
                        var item = db.Products.FirstOrDefault(i => string.Equals(i.ProductName, model.ProductName));
                        if (item == null)
                        {
                            db.Database.ExecuteSqlCommand("EXEC UpdateProduct " +
                                "@ProductName,@Description,@Category,@Price,@OldProductName",
                                new SqlParameter("@ProductName", model.ProductName),
                                new SqlParameter("@Description", model.Description),
                                new SqlParameter("@Category", model.Category),
                                new SqlParameter("@Price", model.Price),
                                new SqlParameter("@OldProductName", model.OldProductName));

                            success = true;
                            message = "Successfully edited";
                        }
                        else
                        {
                            success = false;
                            message = "Such product name is already exist.";
                        }
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("EXEC UpdateProduct " +
                        "@ProductName,@Description,@Category,@Price,@OldProductName",
                        new SqlParameter("@ProductName", model.ProductName),
                        new SqlParameter("@Description", model.Description),
                        new SqlParameter("@Category", model.Category),
                        new SqlParameter("@Price", model.Price),
                        new SqlParameter("@OldProductName", model.OldProductName));

                        success = true;
                        message = "Successfully edited";
                    }
                }

                if (success)
                    db.SaveChanges();
            }

            return Json(new { success, message}, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult ProductAlreadyExistsAsync(string productName, string OldProductName)
        {
            using (ProductContext db = new ProductContext())
            {

                if (!string.IsNullOrEmpty(OldProductName) && string.Equals(productName, OldProductName))
                {
                    // Edit mode and product names (old & new) are equal
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else // For create mode and for case,
                     // when product names are not equal (verify whether a new product is unique)
                {
                    var result = db.Products.FirstOrDefault(i => string.Equals(productName, i.ProductName));

                    return Json(result == null, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Remove(string productName)
        {
            bool success = false;
            string message = "";

            using (ProductContext db = new ProductContext())
            {
                var product = db.Products.FirstOrDefault(i => string.Equals(i.ProductName, productName));
                if (product == null)
                {
                    message = "Element with specified name didn`t find";
                    success = false;
                }
                else
                {
                    db.Products.Remove(product);
                    message = "Successfully removed";
                    success = true;

                    db.SaveChanges();
                }

                return Json(new { success, message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}