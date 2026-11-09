using BE4.Models;
using BE4.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace BE4.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập đã tồn tại chưa
                var existingUser = db.Users.SingleOrDefault(
                    u => u.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError(
                        "Username",
                        "Tên đăng nhập này đã tồn tại!");

                    return View(model);
                }

                // Tạo tài khoản trong bảng User
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserRole = "C"
                };

                db.Users.Add(user);

                // Tạo thông tin khách hàng trong bảng Customer
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    CustomerEmail = model.CustomerEmail,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username
                };

                db.Customers.Add(customer);

                // Lưu cả hai bảng vào database
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password &&
                    u.UserRole == "C");

                if (user != null)
                {
                    // Lưu thông tin đăng nhập vào Session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    // Lưu thông tin xác thực vào Cookie
                    FormsAuthentication.SetAuthCookie(
                        user.Username, false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("",
                        "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult ProfileInfo()
        {
            string username = Session["Username"] as string;

            // Kiểm tra người dùng đã đăng nhập
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            // Lấy thông tin khách hàng đang đăng nhập
            var customer = db.Customers
                             .SingleOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }
        // GET: Account/UpdateProfile
        public ActionResult UpdateProfile()
        {
            string username = Session["Username"] as string;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            var customer = db.Customers
                             .SingleOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            var model = new UpdateProfileVM
            {
                CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail,
                CustomerAddress = customer.CustomerAddress
            };

            return View(model);
        }

        // POST: Account/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(UpdateProfileVM model)
        {
            string username = Session["Username"] as string;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = db.Customers
                             .SingleOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            customer.CustomerPhone = model.CustomerPhone;
            customer.CustomerEmail = model.CustomerEmail;
            customer.CustomerAddress = model.CustomerAddress;

            db.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";

            return RedirectToAction("ProfileInfo");
        }
        // GET: Account/ChangePassword
        public ActionResult ChangePassword()
        {
            string username = Session["Username"] as string;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            string username = Session["Username"] as string;

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.Users.SingleOrDefault(u => u.Username == username);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra mật khẩu hiện tại
            if (user.Password.Trim() != model.CurrentPassword)
            {
                ModelState.AddModelError(
                    "CurrentPassword",
                    "Mật khẩu hiện tại không đúng."
                );

                return View(model);
            }

            // Cập nhật mật khẩu mới
            user.Password = model.NewPassword;

            db.SaveChanges();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";

            return RedirectToAction("ProfileInfo");
        }
    }
}