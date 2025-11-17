using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;

namespace WEB_SALE_LAPTOP.Controllers
{
    // Kế thừa BaseAdminController đã tự động bảo vệ và gán Layout
    public class ProductManagementController : BaseAdminController
    {
        // (BaseAdminController đã cung cấp 'db', không cần khai báo lại)

        // GET: Index
        public ActionResult Index(string search, int? page, int? brandId, int? categoryId)
        {
            var viewModel = new ProductManagementViewModel();

            // 2. Tải (load) dữ liệu lọc (Filters) (Giữ nguyên, đã tốt)
            var brandsList = db.HANGs.ToDictionary(h => h.MAHANG, h => h.TENHANG);
            brandsList.Add(0, "Tất cả Hãng");
            viewModel.Brands = new SelectList(brandsList, "Key", "Value", brandId);

            var categoriesList = db.LOAI_LAPTOP.ToDictionary(l => l.MALOAI, l => l.TENLOAI);
            categoriesList.Add(0, "Tất cả Loại");
            viewModel.Categories = new SelectList(categoriesList, "Key", "Value", categoryId);

            // 3. Bắt đầu Truy vấn (Query) "gốc" (base)
            // SỬA LỖI LINQ: Xóa .OrderBy() khỏi đây
            var laptops = db.LAPTOPs
                            .Include(l => l.HANG)
                            .Include(l => l.LOAI_LAPTOP)
                            .AsQueryable(); // <-- Biến nó thành IQueryable

            // 4. "TIẾN HÓA": Tính toán Thống kê (Giữ nguyên, đã tốt)
            // (Tạm thời tải (load) tất cả để đếm)
            var allProducts = laptops.OrderBy(l => l.TENLAPTOP).ToList();
            viewModel.TongSoSanPham = allProducts.Count();
            viewModel.SanPhamDangBan = allProducts.Count(p => p.TRANGTHAI == true);
            viewModel.SanPhamDaAn = allProducts.Count(p => p.TRANGTHAI == false);

            // 5. "TIẾN HÓA": Áp dụng (Apply) Lọc (Filters) (Giữ nguyên, đã tốt)
            if (!string.IsNullOrEmpty(search))
            {
                laptops = laptops.Where(l => l.TENLAPTOP.Contains(search) || l.CAUHINH.Contains(search));
                viewModel.CurrentSearch = search;
            }
            if (brandId.HasValue && brandId.Value > 0)
            {
                laptops = laptops.Where(l => l.MAHANG == brandId.Value);
                viewModel.CurrentBrandId = brandId.Value;
            }
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                laptops = laptops.Where(l => l.MALOAI == categoryId.Value);
                viewModel.CurrentCategoryId = categoryId.Value;
            }

            // 6. "TIẾN HÓA": Thực hiện Phân trang (Pagination)
            int pageNumber = (page ?? 1);
            int pageSize = 10;

            // SỬA LỖI LINQ: Thêm .OrderBy() VÀO CUỐI CÙNG
            // (Để "biến" (convert) IQueryable -> IOrderedQueryable)
            viewModel.Products = laptops.OrderBy(l => l.TENLAPTOP).ToPagedList(pageNumber, pageSize);

            return View(viewModel);
        }

        // GET: Create
        public ActionResult Create()
        {
            // KHÔNG CẦN GÁN ViewBag.Layout ở đây nữa
            ViewBag.MALOAI = new SelectList(db.LOAI_LAPTOP, "MALOAI", "TENLOAI");
            ViewBag.MAHANG = new SelectList(db.HANGs, "MAHANG", "TENHANG");
            return View();
        }

        // POST: Create
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

                db.LAPTOPs.Add(laptop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MALOAI = new SelectList(db.LOAI_LAPTOP, "MALOAI", "TENLOAI", laptop.MALOAI);
            ViewBag.MAHANG = new SelectList(db.HANGs, "MAHANG", "TENHANG", laptop.MAHANG);
            return View(laptop);
        }

        // GET: Edit
        public ActionResult Edit(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            LAPTOP laptop = db.LAPTOPs.Find(id);
            if (laptop == null) { return HttpNotFound(); }
            ViewBag.MALOAI = new SelectList(db.LOAI_LAPTOP, "MALOAI", "TENLOAI", laptop.MALOAI);
            ViewBag.MAHANG = new SelectList(db.HANGs, "MAHANG", "TENHANG", laptop.MAHANG);
            return View(laptop);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "MALAPTOP,TENLAPTOP,MOTA,CAUHINH,GIA_GOC,GIA_BAN,SOLUONG_TON,MALOAI,MAHANG,TRANGTHAI,HINHANH0,HINHANH1,HINHANH2,HINHANH3")] LAPTOP laptop,
            HttpPostedFileBase HINHANH0_File, HttpPostedFileBase HINHANH1_File,
            HttpPostedFileBase HINHANH2_File, HttpPostedFileBase HINHANH3_File)
        {
            if (ModelState.IsValid)
            {
                string newImg0 = SaveImage(HINHANH0_File);
                if (newImg0 != null) laptop.HINHANH0 = newImg0;
                string newImg1 = SaveImage(HINHANH1_File);
                if (newImg1 != null) laptop.HINHANH1 = newImg1;
                string newImg2 = SaveImage(HINHANH2_File);
                if (newImg2 != null) laptop.HINHANH2 = newImg2;
                string newImg3 = SaveImage(HINHANH3_File);
                if (newImg3 != null) laptop.HINHANH3 = newImg3;

                db.Entry(laptop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MALOAI = new SelectList(db.LOAI_LAPTOP, "MALOAI", "TENLOAI", laptop.MALOAI);
            ViewBag.MAHANG = new SelectList(db.HANGs, "MAHANG", "TENHANG", laptop.MAHANG);
            return View(laptop);
        }

        // GET: Delete
        public ActionResult Delete(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            LAPTOP laptop = db.LAPTOPs.Include(l => l.HANG).Include(l => l.LOAI_LAPTOP)
                                 .FirstOrDefault(l => l.MALAPTOP == id);
            if (laptop == null) { return HttpNotFound(); }
            return View(laptop);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LAPTOP laptop = db.LAPTOPs.Find(id);
            try
            {
                db.LAPTOPs.Remove(laptop);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi đăng ký tài khoản: " + ex.Message);
                ViewBag.Error = "Không thể xóa sản phẩm này vì đã tồn tại trong hóa đơn!";
                db.Entry(laptop).Reference(l => l.HANG).Load();
                db.Entry(laptop).Reference(l => l.LOAI_LAPTOP).Load();
                return View(laptop);
            }
            return RedirectToAction("Index");
        }

        // (Hàm SaveImage - Giữ nguyên)
        private string SaveImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                Directory.CreateDirectory(Server.MapPath("~/Content/images/"));
                file.SaveAs(path);
                return fileName;
            }
            return null;
        }


        public ProductManagementController() : base(maQuyenCanCo: 3)
        {
            // Để trống
        }
    }
}