using BE4.Models;
using BE4.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;


namespace BE4.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private MyStoreEntities db = new MyStoreEntities();
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();

            var products = db.Products.AsQueryable();

            // Tìm kiếm sản phẩm theo tên, mô tả hoặc danh mục
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;

                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDecription.Contains(searchTerm) ||
                    p.Category.CategoryName.Contains(searchTerm)
                );
            }

            // Thông tin phân trang
            int pageNumber = page ?? 1;
            int pageSize = 6;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            // Top 10 sản phẩm bán chạy nhất
            model.FeaturedProducts = products
                .OrderByDescending(p => p.OrderDetails.Count())
                .Take(10)
                .ToList();

            // 20 sản phẩm ít người mua nhất, phân trang 6 sản phẩm/trang
            model.NewProducts = products
                .OrderBy(p => p.OrderDetails.Count())
                .Take(20)
                .ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        // GET: Home/ProductList
        public ActionResult ProductList(int? categoryId)
        {
            if (!categoryId.HasValue)
            {
                return RedirectToAction("Index");
            }

            // Tìm danh mục được chọn
            var category = db.Categories.Find(categoryId.Value);

            if (category == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách sản phẩm thuộc danh mục
            var products = db.Products
                             .Include(p => p.Category)
                             .Where(p => p.CategoryID == categoryId.Value)
                             .ToList();

            ViewBag.CategoryName = category.CategoryName;

            return View(products);
        }
        // GET: Home/ProductDetail
        public ActionResult ProductDetail(int? id, int? quantity, int? page)
        {
            // Kiểm tra mã sản phẩm
            if (id == null)
            {
                return new HttpStatusCodeResult(400);
            }

            // Lấy thông tin sản phẩm
            var product = db.Products
                            .Include(p => p.Category)
                            .Include(p => p.OrderDetails)
                            .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Lấy các sản phẩm cùng danh mục, trừ sản phẩm hiện tại
            var products = db.Products
                             .Where(p =>
                                 p.CategoryID == product.CategoryID &&
                                 p.ProductID != product.ProductID);

            // Thông tin phân trang Top Products
            int pageNumber = page ?? 1;
            int pageSize = 3;

            // Kiểm tra số lượng mua
            int selectedQuantity = quantity.HasValue && quantity.Value > 0
                ? quantity.Value
                : 1;

            // Khởi tạo ViewModel
            var model = new ProductDetailVM();

            model.Product = product;
            model.Quantity = selectedQuantity;
            model.EstimatedValue = product.ProductPrice * selectedQuantity;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            // Danh sách 8 sản phẩm cùng danh mục
            model.RelatedProducts = products
                .OrderBy(p => p.ProductID)
                .Take(8)
                .ToList();

            // Danh sách 8 sản phẩm bán chạy nhất cùng danh mục
            model.TopProducts = products
                .OrderByDescending(p => p.OrderDetails.Count())
                .Take(8)
                .ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        // Hiển thị danh sách danh mục trên Customer Layout
        [ChildActionOnly]
        public ActionResult CategoryMenu()
        {
            var categories = db.Categories
                               .OrderBy(c => c.CategoryName)
                               .ToList();

            return PartialView("_CategoryMenu", categories);
        }
    }
}