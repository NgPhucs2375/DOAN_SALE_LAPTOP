using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;

namespace WEB_SALE_LAPTOP.Controllers
{
    public class AccountController : Controller
    {
        private QL_LAPTOP db = new QL_LAPTOP();

        // (Hàm Register (GET) của bạn đã tốt, giữ nguyên)
        public ActionResult Register()
        {
            return View();
        }

        // ===================================================================
        // REGISTER (POST) - ĐÃ "TIẾN HÓA" SANG EF
        // ===================================================================
        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            string hoten = collection["HOTEN"];
            string email = collection["EMAIL"];
            string sodt = collection["SODT"];
            string diachi = collection["DIACHI"];
            string matkhau = collection["MATKHAU"];
            string matkhau_nhaplai = collection["MATKHAU_NHAPLAI"];

            // (Logic kiểm tra của bạn đã tốt, giữ nguyên)
            if (matkhau != matkhau_nhaplai) { /* ... */ }
            var existingUserByEmail = db.KHACHHANGs.FirstOrDefault(k => k.EMAIL == email);
            if (existingUserByEmail != null) { /* ... */ }
            if (!string.IsNullOrEmpty(sodt)) { /* ... */ }

            KHACHHANG newCustomer = new KHACHHANG
            {
                HOTEN = hoten,
                EMAIL = email,
                SODT = sodt,
                DIACHI = diachi,
                MATKHAU = matkhau, // (LƯU Ý: Bạn nên mã hóa mật khẩu này)
                NGAYTAO = DateTime.Now
            };

            try
            {
                // === SỬA LỖI: DÙNG CÚ PHÁP EF ===
                db.KHACHHANGs.Add(newCustomer); // Thay vì InsertOnSubmit
                db.SaveChanges();               // Thay vì SubmitChanges
                // ================================

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã xảy ra lỗi không xác định. Vui lòng thử lại.";
                return View();
            }
        }

        // (Hàm Login (GET & POST) của bạn đã rất tốt, giữ nguyên)
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection, string returnUrl)
        {
            string email = collection["EMAIL"];
            string matkhau = collection["MATKHAU"];
            var user = db.KHACHHANGs.FirstOrDefault(k => k.EMAIL == email && k.MATKHAU == matkhau);

            if (user != null)
            {
                Session["UserCustomer"] = user;
                Session["UserName"] = user.HOTEN;
                Session["MaKH"] = user.MAKH;

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Error = "Email hoặc Mật khẩu không chính xác.";
                return View();
            }
        }

        // (Hàm Logout của bạn đã tốt, giữ nguyên)
        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

        // (Hàm Dispose của bạn đã tốt, giữ nguyên)
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}