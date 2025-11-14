using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;
using System.Data.Entity;
using System.Net;

namespace WEB_SALE_LAPTOP.Controllers
{
    // Kế thừa "bộ não" Admin (Đã "tiến hóa")
    public class OrderManagementController : BaseAdminController
    {
        // "TIẾN HÓA": Yêu cầu Quyền 2 (Bán hàng) hoặc 1 (Admin)
        public OrderManagementController() : base(maQuyenCanCo: 2)
        {
            // Để trống
        }

        // ===================================================================
        // "TIẾN HÓA PRO" (CẤP 3) - HÀM INDEX MỚI
        // ===================================================================
        // Chấp nhận một "status" (trạng thái) để lọc
        public ActionResult Index(string status = null)
        {
            // 1. Lấy TẤT CẢ hóa đơn (để làm thống kê)
            var allHoadons = db.HOADONs
                                .Include(h => h.KHACHHANG)
                                .Include(h => h.CT_HOADON)
                                .OrderByDescending(h => h.NGAYLAP)
                                .ToList();

            // 2. "TIẾN HÓA" (Requirement #1): Tính toán Thống kê
            var viewModel = new OrderManagementViewModel
            {
                // Tính tổng doanh thu (chỉ từ các đơn "Hoàn thành")
                TongDoanhThu = allHoadons
                                .Where(h => h.TRANGTHAI == "Hoàn thành")
                                .Sum(h => h.TONG_THANHTOAN.GetValueOrDefault(0)),

                TongDonHang = allHoadons.Count(),
                DonChoXuLy = allHoadons.Count(h => h.TRANGTHAI == "Chờ xử lý"),
                DonDangGiao = allHoadons.Count(h => h.TRANGTHAI == "Đang giao"),
                CurrentFilter = status // Ghi nhớ bộ lọc hiện tại
            };

            // 3. "TIẾN HÓA" (Requirement #3): Lọc (Filter) Bảng
            if (string.IsNullOrEmpty(status))
            {
                // Nếu không lọc, hiển thị tất cả
                viewModel.Hoadons = allHoadons;
            }
            else
            {
                // Nếu có lọc (nhấn vào card), chỉ hiển thị các đơn đó
                viewModel.Hoadons = allHoadons
                                    .Where(h => h.TRANGTHAI == status)
                                    .ToList();
            }

            return View(viewModel); // <-- Trả về ViewModel "Pro" mới
        }

        // (Hàm Details và UpdateStatus của bạn đã rất tốt, giữ nguyên)
        public ActionResult Details(int? id)
        {
            // ... (code cũ giữ nguyên) ...
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            HOADON hoadon = db.HOADONs
                .Include(h => h.KHACHHANG)
                .Include(h => h.CT_HOADON.Select(ct => ct.LAPTOP))
                .FirstOrDefault(h => h.MAHD == id);
            if (hoadon == null) { return HttpNotFound(); }
            return View(hoadon);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int maHD, string newStatus)
        {
            // ... (code cũ giữ nguyên) ...
            var hoadon = db.HOADONs.Find(maHD);
            if (hoadon == null) { return RedirectToAction("Index"); }

            hoadon.TRANGTHAI = newStatus;

            if (newStatus == "Đã hủy")
            {
                var chiTietDonHang = db.CT_HOADON.Where(ct => ct.MAHD == maHD).ToList();
                foreach (var item in chiTietDonHang)
                {
                    var sanPham = db.LAPTOPs.Find(item.MALAPTOP);
                    if (sanPham != null)
                    {
                        sanPham.SOLUONG_TON += item.SOLUONG.GetValueOrDefault(0);
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}