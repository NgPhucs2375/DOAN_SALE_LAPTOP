using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;
using System.Data.Entity; // <-- THÊM DÒNG NÀY

namespace WEB_SALE_LAPTOP.Controllers
{
    public class HomeController : Controller
    {
        // THAY ĐỔI 1: Đổi thành 'private' (chuẩn Lập trình Hướng đối tượng)
        private QL_LAPTOP data = new QL_LAPTOP();

        // ========== CÁC HÀM ĐÃ SỬA LỖI (THÊM .Include()) ==========

        public ActionResult Index()
        {
            // THÊM .Include() để lấy Hãng và Loại cho Card sản phẩm
            List<LAPTOP> dsLaptop = data.LAPTOPs
                                        .Include(l => l.HANG)
                                        .Include(l => l.LOAI_LAPTOP)
                                        .Where(l => l.TRANGTHAI == true) // Chỉ hiển thị sản phẩm đang bán
                                        .ToList();
            return View(dsLaptop);
        }

        public ActionResult DanhSachSanPham()
        {
            // THÊM .Include() - Đây là lỗi chúng ta đã bàn lần trước
            List<LAPTOP> dsLaptop = data.LAPTOPs
                                        .Include(l => l.HANG)
                                        .Include(l => l.LOAI_LAPTOP)
                                        .ToList();
            return View(dsLaptop);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // THÊM .Include() để hiển thị thông tin Hãng/Loại ở trang chi tiết
            LAPTOP laptop = data.LAPTOPs
                                .Include(l => l.HANG)
                                .Include(l => l.LOAI_LAPTOP)
                                .FirstOrDefault(l => l.MALAPTOP == id);

            if (laptop == null)
            {
                return HttpNotFound();
            }

            // (Phần code ViewBag của bạn đã rất tốt, giữ nguyên)
            var images = new List<string>();
            if (!string.IsNullOrEmpty(laptop.HINHANH0)) images.Add(laptop.HINHANH0);
            if (!string.IsNullOrEmpty(laptop.HINHANH1)) images.Add(laptop.HINHANH1);
            if (!string.IsNullOrEmpty(laptop.HINHANH2)) images.Add(laptop.HINHANH2);
            if (!string.IsNullOrEmpty(laptop.HINHANH3)) images.Add(laptop.HINHANH3);
            ViewBag.ProductImages = images;

            int maxProducts = 5;
            ViewBag.SameBrandProducts = data.LAPTOPs
                .Where(l => l.MAHANG == laptop.MAHANG && l.MALAPTOP != id)
                .OrderByDescending(l => l.MALAPTOP)
                .Take(maxProducts)
                .ToList();
            ViewBag.SameTypeProducts = data.LAPTOPs
                .Where(l => l.MALOAI == laptop.MALOAI && l.MALAPTOP != id)
                .OrderByDescending(l => l.MALAPTOP)
                .Take(maxProducts)
                .ToList();

            return View(laptop);
        }

        public ActionResult TimKiem(string tuKhoa)
        {
            ViewBag.TuKhoa = tuKhoa;

            List<LAPTOP> ketQuaTimKiem;

            if (string.IsNullOrEmpty(tuKhoa))
            {
                ketQuaTimKiem = new List<LAPTOP>();
            }
            else
            {
                string tuKhoaLower = tuKhoa.ToLower();
                ketQuaTimKiem = data.LAPTOPs
                                    .Include(l => l.HANG) // THÊM .Include()
                                    .Include(l => l.LOAI_LAPTOP) // THÊM .Include()
                                    .Where(s => s.TENLAPTOP.ToLower().Contains(tuKhoaLower))
                                    .ToList();
            }

            // QUAN TRỌNG: Trả về View "Index" (Trang chủ)
            // để hiển thị kết quả tìm kiếm DƯỚI DẠNG CARD SẢN PHẨM
            // thay vì một trang mới.
            return View("Index", ketQuaTimKiem);
        }

        // ========== CÁC HÀM KHÁC (GIỮ NGUYÊN) ==========

        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByParam = "none")] // Cache trong 3600 giây = 1 giờ
        // chỉ dọi csdl cho menu này 1 lần/giờ chứ ko phải mỗi lần load trang
        public ActionResult DSMenu_Hang()
        {
            var dsHang = data.HANGs.ToList();
            return PartialView("_DSMenu_Hang", dsHang);
        }

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

        // THAY ĐỔI 2: Thêm hàm Dispose để đóng kết nối CSDL (Sửa Vấn đề 2)
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                data.Dispose(); // Đóng kết nối 'data'
            }
            base.Dispose(disposing);
        }

        public ActionResult TimKiemTheoLoai(string tenLoai)
        {
            // Đặt tên loại vào ViewBag để View có thể hiển thị
            ViewBag.TieuDeLoc = tenLoai;

            var ketQua = data.LAPTOPs
                            .Include(l => l.HANG)
                            .Include(l => l.LOAI_LAPTOP)
                            .Where(l => l.LOAI_LAPTOP.TENLOAI.ToLower() == tenLoai.ToLower())
                            .ToList();

            // Dùng lại View "Index" (Trang chủ) để hiển thị kết quả
            return View("Index", ketQua);
        }

        public ActionResult LaptopsTheoHang(int maHang)
        {
            var hang = data.HANGs.Find(maHang);
            ViewBag.TieuDeLoc = "Hãng: " + (hang != null ? hang.TENHANG : "");

            var ketQua = data.LAPTOPs
                            .Include(l => l.HANG)
                            .Include(l => l.LOAI_LAPTOP)
                            .Where(l => l.MAHANG == maHang)
                            .ToList();

            return View("Index", ketQua);
        }


        public ActionResult TimKiemTheoGia(decimal? minGia, decimal? maxGia)
        {
            string tieuDe = "Lọc theo giá: ";

            // Xây dựng câu truy vấn
            var query = data.LAPTOPs
                            .Include(l => l.HANG)
                            .Include(l => l.LOAI_LAPTOP);

            if (minGia != null)
            {
                query = query.Where(l => l.GIA_BAN >= minGia);
                tieuDe += $"từ {minGia:N0}đ ";
            }

            if (maxGia != null)
            {
                query = query.Where(l => l.GIA_BAN <= maxGia);
                tieuDe += $"đến {maxGia:N0}đ";
            }

            ViewBag.TieuDeLoc = tieuDe;
            var ketQua = query.ToList();

            return View("Index", ketQua);
        }

        public ActionResult TimKiemTheoCPU(string cpuKey) // <-- Đổi tên tham số
        {
            // Đặt tiêu đề (dựa trên key)
            if (cpuKey == "i5") ViewBag.TieuDeLoc = "CPU: Intel Core i5";
            else if (cpuKey == "i7") ViewBag.TieuDeLoc = "CPU: Intel Core i7";
            else if (cpuKey == "ryzen 5") ViewBag.TieuDeLoc = "CPU: AMD Ryzen 5";
            else if (cpuKey == "ryzen 7") ViewBag.TieuDeLoc = "CPU: AMD Ryzen 7";
            else ViewBag.TieuDeLoc = "CPU: " + cpuKey;

            string cpuSearchTerm = cpuKey.ToLower(); // <-- Đây chính là "i5" hoặc "ryzen 7"

            var ketQua = data.LAPTOPs
                            .Include(l => l.HANG)
                            .Include(l => l.LOAI_LAPTOP)
                            // Logic tìm kiếm "mạnh mẽ" (robust)
                            .Where(l => l.CAUHINH.ToLower().Contains(cpuSearchTerm))
                            .ToList();

            return View("Index", ketQua);
        }
        [HttpPost]
        public ActionResult SubscribeNewsletter(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Vui lòng nhập email." });
            }

            // (Logic tương lai: Bạn sẽ lưu email này vào CSDL)
            // db.NEWSLETTER_SUBS.Add(new NEWSLETTER_SUB { Email = email });
            // db.SaveChanges();

            // Tạm thời, chúng ta chỉ trả về "Thành công"
            return Json(new { success = true, message = "Đăng ký thành công! Cảm ơn bạn." });
        }
    }
}