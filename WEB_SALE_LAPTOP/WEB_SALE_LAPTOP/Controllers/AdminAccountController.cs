using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;
using System.Data.Entity;

namespace WEB_SALE_LAPTOP.Controllers
{
    public class AdminAccountController : Controller
    {
        private QL_LAPTOP db = new QL_LAPTOP();

        public ActionResult Login()
        {
            return View("Login"); 
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            string tendn = collection["TENDN"];
            string matkhau = collection["MATKHAU"];

            // 1. Tìm nhân viên theo User/Pass
            var nv = db.NHANVIENs.FirstOrDefault(n => n.TENDN == tendn && n.MATKHAU == matkhau);

            if (nv != null)
            {
                // 2. Lấy danh sách TẤT CẢ quyền của nhân viên này
                var dsQuyen = db.Database.SqlQuery<int>(
                    "SELECT MAQUYEN FROM CAPQUYEN WHERE MANV = {0}", nv.MANV
                ).ToList();

                // 3. Kiểm tra: Nếu có ít nhất 1 quyền thì cho vào
                if (dsQuyen.Count > 0)
                {
                    // Lưu Session
                    Session["AdminUser"] = nv;
                    Session["AdminName"] = nv.HOTEN;
                    Session["AdminQuyen"] = dsQuyen; 

                    // Chuyển hướng thông minh dựa trên quyền
                    if (dsQuyen.Contains(1)) // Admin
                    {
                        return RedirectToAction("Index", "Revenue"); // Admin vào xem báo cáo
                    }
                    else if (dsQuyen.Contains(2)) // Bán hàng
                    {
                        return RedirectToAction("Index", "OrderManagement"); // Bán hàng vào xem đơn
                    }
                    else if (dsQuyen.Contains(3)) // Thủ kho
                    {
                        return RedirectToAction("Index", "ProductManagement"); // Kho vào xem sản phẩm
                    }
                    else
                    {
                        return RedirectToAction("Index", "ProductManagement"); // Mặc định
                    }
                }
                else
                {
                    ViewBag.Error = "Tài khoản của bạn chưa được cấp quyền truy cập!";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Remove("AdminUser");
            Session.Remove("AdminName");
            return RedirectToAction("Login", "AdminAccount");
        }

   
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