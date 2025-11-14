using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; // <-- Đã có
using WEB_SALE_LAPTOP.Models;

namespace WEB_SALE_LAPTOP.Controllers
{
    public class ProductManagementController : Controller
    {
        private QL_LAPTOP db = new QL_LAPTOP();

        // (Hàm OnActionExecuting (Bảo mật) của bạn đã rất tốt, giữ nguyên)
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["AdminUser"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "AdminAccount", action = "Login" }
                    )
                );
            }
            base.OnActionExecuting(filterContext);
        }

        // (Hàm Index (GET) của bạn đã rất tốt, giữ nguyên)
        // Làm phân quyền để admmin mới truy cập được   
        public ActionResult Index()
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml"; // <-- GÁN LAYOUT MỚI
            var laptops = db.LAPTOPs.Include(l => l.HANG).Include(l => l.LOAI_LAPTOP);
            return View(laptops.ToList());
        }

        // (Hàm Create (GET) của bạn đã rất tốt, giữ nguyên)
        public ActionResult Create()
        {
            ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml"; // <-- GÁN LAYOUT MỚI
            ViewBag.MALOAI = new SelectList(db.LOAI_LAPTOP, "MALOAI", "TENLOAI");
            ViewBag.MAHANG = new SelectList(db.HANGs, "MAHANG", "TENHANG");
            return View();
        }

        // ===================================================================
        // CREATE (POST) - ĐÃ "TIẾN HÓA" SANG EF
        // ===================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TENLAPTOP,MOTA,CAUHINH,GIA_GOC,GIA_BAN,SOLUONG_TON,MALOAI,MAHANG,TRANGTHAI")] LAPTOP laptop,
            HttpPostedFileBase HINHANH0_File, HttpPostedFileBase HINHANH1_File,
            HttpPostedFileBase HINHANH2_File, HttpPostedFileBase HINHANH3_File)
        {
            if (ModelState.IsValid)
            {
                laptop.HINHANH0 = SaveImage(HINHANH0_File);
                laptop.HINHANH1 = SaveImage(HINHANH1_File);
                laptop.HINHANH2 = SaveImage(HINHANH2_File);
                laptop.HINHANH3 = SaveImage(HINHANH3_File);

                // === SỬA LỖI: DÙNG CÚ PHÁP EF ===
                db.LAPTOPs.Add(laptop);     // Thay vì InsertOnSubmit
                db.SaveChanges();           // Thay vì SubmitChanges
                // ================================

                return RedirectToAction("Index");
            }

            ViewBag.MALOAI = new SelectList(db.LOAI_LAPTOP, "MALOAI", "TENLOAI", laptop.MALOAI);
            ViewBag.MAHANG = new SelectList(db.HANGs, "MAHANG", "TENHANG", laptop.MAHANG);
            return View(laptop);
        }

        // (Hàm Edit (GET) của bạn đã rất tốt, giữ nguyên)
        public ActionResult Edit(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            LAPTOP laptop = db.LAPTOPs.Find(id);
            if (laptop == null) { return HttpNotFound(); }
            ViewBag.MALOAI = new SelectList(db.LOAI_LAPTOP, "MALOAI", "TENLOAI", laptop.MALOAI);
            ViewBag.MAHANG = new SelectList(db.HANGs, "MAHANG", "TENHANG", laptop.MAHANG);
            return View(laptop);
        }

        // ===================================================================
        // EDIT (POST) - ĐÃ "TIẾN HÓA" SANG EF
        // ===================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "MALAPTOP,TENLAPTOP,MOTA,CAUHINH,GIA_GOC,GIA_BAN,SOLUONG_TON,MALOAI,MAHANG,TRANGTHAI,HINHANH0,HINHANH1,HINHANH2,HINHANH3")] LAPTOP laptop,
            HttpPostedFileBase HINHANH0_File, HttpPostedFileBase HINHANH1_File,
            HttpPostedFileBase HINHANH2_File, HttpPostedFileBase HINHANH3_File)
        {
            if (ModelState.IsValid)
            {
                // (Logic SaveImage của bạn đã tốt, giữ nguyên)
                string newImg0 = SaveImage(HINHANH0_File);
                if (newImg0 != null) laptop.HINHANH0 = newImg0;
                string newImg1 = SaveImage(HINHANH1_File);
                if (newImg1 != null) laptop.HINHANH1 = newImg1;
                string newImg2 = SaveImage(HINHANH2_File);
                if (newImg2 != null) laptop.HINHANH2 = newImg2;
                string newImg3 = SaveImage(HINHANH3_File);
                if (newImg3 != null) laptop.HINHANH3 = newImg3;

                // === SỬA LỖI: DÙNG CÚ PHÁP EF ===
                db.Entry(laptop).State = EntityState.Modified; // Thay vì UpdateOnSubmit
                db.SaveChanges();                                // Thay vì SubmitChanges
                // ================================

                return RedirectToAction("Index");
            }

            ViewBag.MALOAI = new SelectList(db.LOAI_LAPTOP, "MALOAI", "TENLOAI", laptop.MALOAI);
            ViewBag.MAHANG = new SelectList(db.HANGs, "MAHANG", "TENHANG", laptop.MAHANG);
            return View(laptop);
        }

        // (Hàm Delete (GET) của bạn đã rất tốt, giữ nguyên)
        public ActionResult Delete(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            LAPTOP laptop = db.LAPTOPs.Include(l => l.HANG).Include(l => l.LOAI_LAPTOP)
                                 .FirstOrDefault(l => l.MALAPTOP == id);
            if (laptop == null) { return HttpNotFound(); }
            return View(laptop);
        }

        // ===================================================================
        // DELETE (POST) - ĐÃ "TIẾN HÓA" SANG EF
        // ===================================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LAPTOP laptop = db.LAPTOPs.Find(id);

            try
            {
                // === SỬA LỖI: DÙNG CÚ PHÁP EF ===
                db.LAPTOPs.Remove(laptop);  // Thay vì DeleteOnSubmit
                db.SaveChanges();           // Thay vì SubmitChanges
                // ================================
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể xóa sản phẩm này vì đã tồn tại trong hóa đơn!";

                // (Logic catch của bạn đã dùng EF, giữ nguyên)
                db.Entry(laptop).Reference(l => l.HANG).Load();
                db.Entry(laptop).Reference(l => l.LOAI_LAPTOP).Load();
                return View(laptop);
            }
            return RedirectToAction("Index");
        }

        // (Hàm Dispose của bạn đã rất tốt, giữ nguyên)
        protected override void Dispose(bool disposing)
        {
            if (disposing) { db.Dispose(); }
            base.Dispose(disposing);
        }

        // (Hàm SaveImage của bạn đã rất tốt, giữ nguyên)
        private string SaveImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                // Sửa đường dẫn nếu cần
                var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);

                // (Kiểm tra và tạo thư mục nếu chưa có)
                Directory.CreateDirectory(Server.MapPath("~/Content/images/"));

                file.SaveAs(path);
                return fileName;
            }
            return null;
        }
    }
}