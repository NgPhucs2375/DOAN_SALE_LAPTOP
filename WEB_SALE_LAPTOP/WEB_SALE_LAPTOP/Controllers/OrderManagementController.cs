using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;
using System.Data.Entity;
using System.Net;
using OfficeOpenXml; 

namespace WEB_SALE_LAPTOP.Controllers
{
    public class OrderManagementController : BaseAdminController
    {
        public OrderManagementController() : base(maQuyenCanCo: 2) { }

        // INDEX: Danh sách đơn hàng & Thống kê
        public ActionResult Index(string status = null)
        {
            var allHoadons = db.HOADONs
                                .Include(h => h.KHACHHANG)
                                .Include(h => h.CT_HOADON)
                                .OrderByDescending(h => h.NGAYLAP)
                                .ToList();

            var viewModel = new OrderManagementViewModel
            {
                TongDoanhThu = allHoadons.Where(h => h.TRANGTHAI == "Hoàn thành").Sum(h => h.TONG_THANHTOAN.GetValueOrDefault(0)),
                TongDonHang = allHoadons.Count(),
                DonChoXuLy = allHoadons.Count(h => h.TRANGTHAI == "Chờ xử lý"),
                DonDangGiao = allHoadons.Count(h => h.TRANGTHAI == "Đang giao"),
                CurrentFilter = status
            };

            if (string.IsNullOrEmpty(status))
            {
                viewModel.Hoadons = allHoadons;
            }
            else
            {
                viewModel.Hoadons = allHoadons.Where(h => h.TRANGTHAI == status).ToList();
            }

            return View(viewModel);
        }

        // DETAILS: Chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            HOADON hoadon = db.HOADONs
                .Include(h => h.KHACHHANG)
                .Include(h => h.CT_HOADON.Select(ct => ct.LAPTOP))
                .FirstOrDefault(h => h.MAHD == id);
            if (hoadon == null) { return HttpNotFound(); }
            return View(hoadon);
        }

        // UPDATE STATUS: Cập nhật trạng thái
        [HttpPost]
        public ActionResult UpdateStatus(int maHD, string newStatus)
        {
            var hoadon = db.HOADONs.Find(maHD);
            if (hoadon == null) { return RedirectToAction("Index"); }

            hoadon.TRANGTHAI = newStatus;

            // Hoàn trả tồn kho nếu hủy đơn
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

        // EXPORT EXCEL: Xuất báo cáo
        public void ExportToExcel()
        {
            var listOrder = db.HOADONs.OrderByDescending(x => x.NGAYLAP).ToList();

            // CẤU HÌNH LICENSE CHO EPPLUS 6.2.4
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("BaoCaoDonHang");

                // Header
                ws.Cells["A1"].Value = "Mã Đơn";
                ws.Cells["B1"].Value = "Ngày Đặt";
                ws.Cells["C1"].Value = "Khách Hàng";
                ws.Cells["D1"].Value = "Tổng Tiền";
                ws.Cells["E1"].Value = "Trạng Thái";
                ws.Cells["A1:E1"].Style.Font.Bold = true;

                // Data
                int rowStart = 2;
                foreach (var item in listOrder)
                {
                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.MAHD;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.NGAYLAP.HasValue ? item.NGAYLAP.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.SDT_GIAO;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.TONG_THANHTOAN;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "#,##0"; // Định dạng số
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.TRANGTHAI;
                    rowStart++;
                }

                ws.Cells["A:AZ"].AutoFitColumns();

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=BaoCaoDonHang.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }
    }
}