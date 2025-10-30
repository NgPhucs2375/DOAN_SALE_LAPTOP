using DOAN_SALE_LAPTOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN_SALE_LAPTOP.Controllers
{
    public class GioHangController : Controller
    {
        DB db = new DB();

        public GioHangController()
        {
            try
            {
                db.Lap_ListLaptop();
            }
            catch { }
        }

        // Lấy giỏ hàng từ Session (dùng model GioHang thống nhất với Layout, Home)
        private GioHang GetGioHang()
        {
            var gioHang = Session["GioHang"] as GioHang;
            if (gioHang == null)
            {
                gioHang = new GioHang();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }

        private void SaveGioHang(GioHang gioHang)
        {
            Session["GioHang"] = gioHang;
        }

        private void ClearGioHang()
        {
            Session["GioHang"] = new GioHang();
        }

        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var gioHang = GetGioHang();
            return View(gioHang);
        }

        // Thêm sản phẩm vào giỏ
        [HttpPost]
        public ActionResult ThemVaoGio(string id)
        {
            var laptop = db.dsLaptop.FirstOrDefault(x => x.IDLaptop == id);
            if (laptop == null)
                return RedirectToAction("Index", "Home");

            var gioHang = GetGioHang();
            var sp = gioHang.Items.FirstOrDefault(x => x.IDLaptop == id);
            if (sp != null)
            {
                sp.Quantity++;
            }
            else
            {
                gioHang.Items.Add(new GioHangItem
                {
                    IDLaptop = laptop.IDLaptop,
                    NameLaptop = laptop.NameLaptop,
                    PriceLaptop = laptop.PriceLaptop,
                    GraphLaptop = laptop.GraphLaptop,
                    HinhAnh = laptop.HinhAnh,
                    Quantity = 1
                });
            }

            SaveGioHang(gioHang);
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToAction("Index", "Home");
        }

        // Mua ngay 1 sản phẩm -> xóa giỏ hiện tại và chuyển tới trang thanh toán
        [HttpPost]
        public ActionResult MuaNgay(string id)
        {
            var laptop = db.dsLaptop.FirstOrDefault(l => l.IDLaptop == id);
            if (laptop == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var gioHang = GetGioHang();
            gioHang.Items.Clear();
            gioHang.Items.Add(new GioHangItem
            {
                IDLaptop = laptop.IDLaptop,
                NameLaptop = laptop.NameLaptop,
                PriceLaptop = laptop.PriceLaptop,
                GraphLaptop = laptop.GraphLaptop,
                HinhAnh = laptop.HinhAnh,
                Quantity = 1
            });
            SaveGioHang(gioHang);

            return RedirectToAction("ThanhToan", "Home");
        }

        // Cập nhật số lượng (AJAX)
        [HttpPost]
        public ActionResult CapNhatSoLuong(string id, int quantity)
        {
            try
            {
                var gioHang = GetGioHang();
                var item = gioHang.Items.FirstOrDefault(i => i.IDLaptop == id);

                if (item != null)
                {
                    if (quantity <= 0)
                    {
                        gioHang.Items.Remove(item);
                    }
                    else
                    {
                        item.Quantity = quantity;
                    }
                    SaveGioHang(gioHang);
                }

                return Json(new
                {
                    success = true,
                    totalAmount = gioHang.TotalAmount.ToString("N0"),
                    itemTotal = item != null ? item.TotalPrice.ToString("N0") : null,
                    totalItems = gioHang.TotalItems
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Cập nhật số lượng (non-AJAX form)
        [HttpPost]
        public ActionResult CapNhat(string id, int quantity)
        {
            var gioHang = GetGioHang();
            var item = gioHang.Items.FirstOrDefault(i => i.IDLaptop == id);
            if (item != null)
            {
                item.Quantity = quantity <= 0 ? 1 : quantity;
                SaveGioHang(gioHang);
            }
            return RedirectToAction("Index");
        }

        // Xóa 1 sản phẩm khỏi giỏ
        [HttpPost]
        public ActionResult XoaKhoiGioHang(string id)
        {
            var gioHang = GetGioHang();
            var item = gioHang.Items.FirstOrDefault(i => i.IDLaptop == id);

            if (item != null)
            {
                gioHang.Items.Remove(item);
                SaveGioHang(gioHang);
                TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            }

            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ
        [HttpPost]
        public ActionResult XoaGioHang()
        {
            ClearGioHang();
            TempData["SuccessMessage"] = "Đã xóa toàn bộ giỏ hàng!";
            return RedirectToAction("Index");
        }
    }
}