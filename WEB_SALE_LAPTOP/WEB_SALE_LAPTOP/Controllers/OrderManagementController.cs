using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;
using System.Data.Entity;
using System.Net;
using OfficeOpenXml; // Thư viện EPPlus để xuất Excel

namespace WEB_SALE_LAPTOP.Controllers
{

    public class OrderManagementController : BaseAdminController
    {
        //Yêu cầu Quyền 2 (Bán hàng) hoặc 1 (Admin)
        public OrderManagementController() : base(maQuyenCanCo: 2)
        {
            // Để trống
        }

        public ActionResult Index(string status = null)
        {
            // 1. Lấy TẤT CẢ hóa đơn (để làm thống kê)
            var allHoadons = db.HOADONs
                                .Include(h => h.KHACHHANG)
                                .Include(h => h.CT_HOADON)
                                .OrderByDescending(h => h.NGAYLAP)
                                .ToList();

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

        public void ExportToExcel()
        {
            var listOrder = db.HOADONs.OrderByDescending(x => x.NGAYLAP).ToList();

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Bắt buộc với bản mới
            // Không cần nuaữ tự thêm vô web.cònig rôig bản mới nhất không cần khai báo lại

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DanhSachDonHang");

                // Tạo tiêu đề cột
                ws.Cells["A1"].Value = "Mã Đơn";
                ws.Cells["B1"].Value = "Ngày Đặt";
                ws.Cells["C1"].Value = "Khách Hàng";
                ws.Cells["D1"].Value = "Tổng Tiền";
                ws.Cells["E1"].Value = "Trạng Thái";

                // Đổ dữ liệu
                int rowStart = 2;
                foreach (var item in listOrder)
                {
                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.MAHD;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.NGAYLAP.Value.ToString("dd/MM/yyyy");
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.SDT_GIAO; // Hoặc tên KH
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.TONG_THANHTOAN;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.TRANGTHAI;
                    rowStart++;
                }

                // Tự động dãn cột
                ws.Cells["A:AZ"].AutoFitColumns();

                // Xuất file
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=BaoCaoDonHang.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }
    }
}