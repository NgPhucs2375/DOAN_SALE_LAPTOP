using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;
using System.Data.Entity;
namespace WEB_SALE_LAPTOP.Controllers
{
    public class AccountController : Controller
    {
        private QL_LAPTOP db = new QL_LAPTOP();

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            string hoten = collection["HOTEN"];
            string email = collection["EMAIL"];
            string sodt = collection["SODT"];
            string diachi = collection["DIACHI"];
            string matkhau = collection["MATKHAU"];
            string matkhau_nhaplai = collection["MATKHAU_NHAPLAI"];

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
                MATKHAU = matkhau, 
                NGAYTAO = DateTime.Now
            };

            try
            {
                
                db.KHACHHANGs.Add(newCustomer); 
                db.SaveChanges();       
                

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã xảy ra lỗi không xác định. Vui lòng thử lại.";
                return View();
            }
        }

       
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

        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Profile()
        {
            if (Session["UserCustomer"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = Session["UserCustomer"] as KHACHHANG;
            int khachHangID = customer.MAKH;

            // 3. Tạo "Bộ não" (Brain) ViewModel
            var viewModel = new CustomerProfileViewModel
            {
                // 4. Lấy thông tin khách hàng (mới nhất từ CSDL)
                CustomerInfo = db.KHACHHANGs.Find(khachHangID),

                OrderHistory = db.HOADONs
                                .Where(h => h.MAKH == khachHangID)
                                .OrderByDescending(h => h.NGAYLAP)
                                .Take(10) // (Giới hạn 10 đơn hàng gần nhất)
                                .ToList()
            };

            return View(viewModel); 
        }

        [HttpPost]
        public ActionResult UpdateProfile(KHACHHANG customerData)
        {
            // 1. Bảo vệ (Secure)
            if (Session["UserCustomer"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var customerInDb = db.KHACHHANGs.Find(customerData.MAKH);
                if (customerInDb == null)
                {
                    return HttpNotFound();
                }

                // 3. Cập nhật các trường (fields) được phép
                customerInDb.HOTEN = customerData.HOTEN;
                customerInDb.SODT = customerData.SODT;
                customerInDb.DIACHI = customerData.DIACHI;
                // (Không cho phép đổi Email hoặc Mật khẩu ở đây)

                // 4. Lưu vào CSDL
                db.SaveChanges();

                Session["UserCustomer"] = customerInDb;
                Session["UserName"] = customerInDb.HOTEN;
            }
            catch (Exception ex)
            {
                // (Xử lý lỗi)
            }

            return RedirectToAction("Profile");
        }


   

    }
}