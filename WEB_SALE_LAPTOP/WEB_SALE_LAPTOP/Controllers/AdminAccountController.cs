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
            return View("Login"); // <-- Sửa ở đây
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            string tendn = collection["TENDN"];
            string matkhau = collection["MATKHAU"];
            var nv = db.NHANVIENs.FirstOrDefault(n => n.TENDN == tendn && n.MATKHAU == matkhau);

            if (nv == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc Mật khẩu không đúng.";
                return View();
            }

            // (Logic kiểm tra Admin của bạn đã tốt, giữ nguyên)
            bool isAdmin = db.Database.SqlQuery<int>(
                "SELECT COUNT(*) FROM CAPQUYEN WHERE MANV = {0} AND MAQUYEN = 1", nv.MANV
            ).FirstOrDefault() > 0;

            if (isAdmin) // (Hoặc bạn có thể xóa check 'isAdmin' nếu muốn NV nào cũng vào được)
            {
                // LẤY DANH SÁCH QUYỀN CỦA NHÂN VIÊN
                List<int> dsQuyen = db.Database.SqlQuery<int>(
                                 "SELECT MAQUYEN FROM CAPQUYEN WHERE MANV = {0}", nv.MANV
                             ).ToList();

                // LƯU VÀO SESSION
                Session["AdminUser"] = nv;
                Session["AdminName"] = nv.HOTEN;
                Session["AdminQuyen"] = dsQuyen; // <-- QUAN TRỌNG NHẤT

                return RedirectToAction("Index", "ProductManagement");
            }
            else
            {
                ViewBag.Error = "Bạn không có quyền truy cập trang quản trị.";
                return View();
            }
        }

        // (Hàm Logout của bạn đã tốt, giữ nguyên)
        public ActionResult Logout()
        {
            Session.Remove("AdminUser");
            Session.Remove("AdminName");
            return RedirectToAction("Login", "AdminAccount");
        }

        // ===================================================================
        // NÂNG CẤP: Thêm hàm Dispose (Quản lý CSDL)
        // ===================================================================
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