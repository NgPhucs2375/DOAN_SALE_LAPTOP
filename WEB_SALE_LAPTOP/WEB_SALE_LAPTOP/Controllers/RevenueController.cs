using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;
using System.Data.Entity; // Để dùng .Include()

namespace WEB_SALE_LAPTOP.Controllers
{
    // Chỉ Admin (Quyền 1) mới được xem thống kê tiền nong
    public class RevenueController : BaseAdminController
    {
        public RevenueController() : base(maQuyenCanCo: 1) { }

        // GET: Revenue
        public ActionResult Index()
        {
            // Lấy năm hiện tại để mặc định thống kê
            int currentYear = DateTime.Now.Year;
            ViewBag.CurrentYear = currentYear;

            return View();
        }

        // API: Lấy dữ liệu biểu đồ doanh thu theo 12 tháng
        [HttpGet]
        public JsonResult GetRevenueData(int year)
        {
            try
            {
                // Lấy các đơn hàng đã HOÀN THÀNH hoặc ĐÃ THANH TOÁN trong năm được chọn
                var orders = db.HOADONs
                    .Where(h => h.NGAYLAP.Value.Year == year &&
                               (h.TRANGTHAI == "Hoàn thành" || h.TRANGTHAI.Contains("Đã thanh toán")))
                    .ToList();

                // Mảng chứa doanh thu 12 tháng (Mặc định là 0)
                decimal[] monthlyRevenue = new decimal[12];

                foreach (var order in orders)
                {
                    if (order.NGAYLAP.HasValue)
                    {
                        int monthIndex = order.NGAYLAP.Value.Month - 1; // Tháng 1 là index 0
                        monthlyRevenue[monthIndex] += order.TONG_THANHTOAN.GetValueOrDefault(0);
                    }
                }

                return Json(new { success = true, data = monthlyRevenue, year = year }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Lấy Top 5 sản phẩm bán chạy nhất
        [HttpGet]
        public JsonResult GetTopProducts()
        {
            try
            {

                var topProducts = db.CT_HOADON
                    .Where(ct => ct.HOADON.TRANGTHAI == "Hoàn thành" || ct.HOADON.TRANGTHAI.Contains("Đã thanh toán"))
                    .GroupBy(ct => new { ct.MALAPTOP, ct.LAPTOP.TENLAPTOP })
                    .Select(g => new
                    {
                        TenLaptop = g.Key.TENLAPTOP,
                        SoLuongBan = g.Sum(x => x.SOLUONG)
                    })
                    .OrderByDescending(x => x.SoLuongBan)
                    .Take(5)
                    .ToList();

                return Json(new { success = true, data = topProducts }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}