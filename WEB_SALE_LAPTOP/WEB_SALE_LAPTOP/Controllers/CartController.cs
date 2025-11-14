using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;
using System.Data.Entity; // <-- Đảm bảo bạn có dòng này

namespace WEB_SALE_LAPTOP.Controllers
{
    public class CartController : Controller
    {
        private QL_LAPTOP db = new QL_LAPTOP();
        private const string CartSession = "CartSession";

        // ===================================================================
        // SỬA LỖI: Thêm lại 2 hàm helper đã mất
        // ===================================================================
        private List<CartItem> GetCart()
        {
            List<CartItem> cart = Session[CartSession] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session[CartSession] = cart;
            }
            return cart;
        }

        private void SaveCart(List<CartItem> cart)
        {
            Session[CartSession] = cart;
        }

        // ===================================================================
        // CÁC HÀM CÒN LẠI (ĐÃ CHUẨN EF)
        // ===================================================================

        // GET: /Cart/Index
        public ActionResult Index()
        {
            List<CartItem> cart = GetCart();
            ViewBag.TongTien = cart.Sum(item => item.ThanhTien);
            ViewBag.MaVoucher = Session["MaVoucher"];
            ViewBag.SoTienGiam = Session["SoTienGiam"] ?? 0m;
            ViewBag.TongThanhToan = ViewBag.TongTien - (ViewBag.SoTienGiam ?? 0m);

            return View(cart);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public ActionResult AddToCart(int maLaptop, int? soLuong)
        {
            int soLuongThucTe = soLuong.GetValueOrDefault(1);
            if (soLuongThucTe <= 0) soLuongThucTe = 1;

            List<CartItem> cart = GetCart();
            CartItem item = cart.Find(p => p.MaLaptop == maLaptop);

            if (item != null)
            {
                item.SoLuong += soLuongThucTe;
            }
            else
            {
                LAPTOP laptop = db.LAPTOPs.Find(maLaptop);
                if (laptop != null)
                {
                    // Dùng class CartItem của bạn
                    cart.Add(new CartItem(maLaptop, laptop, soLuongThucTe));
                }
            }
            SaveCart(cart);
            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: /Cart/UpdateItem
        [HttpPost]
        public ActionResult UpdateItem(int maLaptop, int soLuong)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.Find(p => p.MaLaptop == maLaptop);
            if (item != null)
            {
                if (soLuong > 0 && soLuong <= 100)
                {
                    item.SoLuong = soLuong;
                }
                else if (soLuong <= 0)
                {
                    cart.Remove(item);
                }
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // GET: /Cart/RemoveItem
        public ActionResult RemoveItem(int maLaptop)
        {
            List<CartItem> cart = GetCart();
            cart.RemoveAll(p => p.MaLaptop == maLaptop);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // POST: /Cart/ApplyVoucher
        [HttpPost]
        public ActionResult ApplyVoucher(string maVoucher)
        {
            VOUCHER voucher = db.VOUCHERs.FirstOrDefault(v => v.MAVOUCHER == maVoucher);
            List<CartItem> cart = GetCart();
            decimal tongTienHang = cart.Sum(item => item.ThanhTien);

            Session["MaVoucher"] = null;
            Session["SoTienGiam"] = 0m;

            if (voucher == null) { TempData["VoucherError"] = "Mã voucher không tồn tại."; }
            else if (voucher.NGAYKETTHUC < DateTime.Now) { TempData["VoucherError"] = "Voucher đã hết hạn."; }
            else if (voucher.DA_DUNG >= voucher.SOLUONG_DUNG) { TempData["VoucherError"] = "Voucher đã hết lượt sử dụng."; }
            else if (tongTienHang < voucher.DONHANG_TOITHIEU) { TempData["VoucherError"] = $"Voucher này chỉ áp dụng cho đơn hàng từ {voucher.DONHANG_TOITHIEU:N0}đ."; }
            else // Voucher hợp lệ
            {
                decimal soTienGiam = 0;
                if (voucher.LOAI_GIAMGIA == "PHANTRAM")
                {
                    soTienGiam = (tongTienHang * voucher.GIATRI) / 100;
                    if (voucher.GIAM_TOIDA > 0 && soTienGiam > voucher.GIAM_TOIDA)
                    {
                        soTienGiam = voucher.GIAM_TOIDA.Value;
                    }
                }
                else { soTienGiam = voucher.GIATRI; }

                Session["MaVoucher"] = voucher.MAVOUCHER;
                Session["SoTienGiam"] = soTienGiam;
                TempData["VoucherSuccess"] = "Áp dụng voucher thành công!";
            }
            return RedirectToAction("Index");
        }

        // GET: /Cart/Checkout
        public ActionResult Checkout()
        {
            if (Session["UserCustomer"] == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Cart") });
            }
            List<CartItem> cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            ViewBag.TongTien = cart.Sum(item => item.ThanhTien);
            ViewBag.MaVoucher = Session["MaVoucher"];
            ViewBag.SoTienGiam = Session["SoTienGiam"] ?? 0m;
            ViewBag.TongThanhToan = ViewBag.TongTien - (ViewBag.SoTienGiam ?? 0m);

            KHACHHANG kh = Session["UserCustomer"] as KHACHHANG;
            if (kh != null)
            {
                ViewBag.HoTen = kh.HOTEN;
                ViewBag.SoDT = kh.SODT;
                ViewBag.DiaChi = kh.DIACHI;
            }
            return View(cart);
        }

        // POST: /Cart/PlaceOrder
        [HttpPost]
        public ActionResult PlaceOrder(FormCollection collection)
        {
            string sdt_giao = collection["SDT_GIAO"];
            string diachi_giao = collection["DIACHI_GIAO"];

            if (string.IsNullOrEmpty(sdt_giao) || string.IsNullOrEmpty(diachi_giao))
            {
                TempData["CheckoutError"] = "Vui lòng nhập đầy đủ Số điện thoại và Địa chỉ giao hàng.";
                return RedirectToAction("Checkout");
            }

            List<CartItem> cart = GetCart();
            string maVoucher = Session["MaVoucher"] as string;
            decimal soTienGiam = (decimal)(Session["SoTienGiam"] ?? 0m);
            decimal tongTienHang = cart.Sum(item => item.ThanhTien);
            KHACHHANG kh = Session["UserCustomer"] as KHACHHANG;

            HOADON hoadon = new HOADON
            {
                NGAYLAP = DateTime.Now,
                DIACHI_GIAO = diachi_giao,
                SDT_GIAO = sdt_giao,
                TONGTIEN_HANG = tongTienHang,
                MAVOUCHER = maVoucher,
                SOTIEN_GIAM_VOUCHER = soTienGiam,
                TONG_THANHTOAN = tongTienHang - soTienGiam,
                TRANGTHAI = "Chờ xử lý",
                MAKH = (kh != null) ? (int?)kh.MAKH : null
            };

            db.HOADONs.Add(hoadon);
            db.SaveChanges();

            foreach (var item in cart)
            {
                CT_HOADON ct = new CT_HOADON
                {
                    MAHD = hoadon.MAHD,
                    MALAPTOP = item.MaLaptop,
                    DONGIA = item.DonGia,
                    SOLUONG = item.SoLuong,
                    THANHTIEN = item.ThanhTien
                };

                // ===================================================================
                // SỬA LỖI: Tên đúng là 'CT_HOADON' (số ít)
                // ===================================================================
                db.CT_HOADON.Add(ct);

                LAPTOP sp = db.LAPTOPs.Find(item.MaLaptop);
                if (sp != null)
                {
                    sp.SOLUONG_TON -= item.SoLuong;
                }
            }

            if (maVoucher != null)
            {
                VOUCHER v = db.VOUCHERs.FirstOrDefault(x => x.MAVOUCHER == maVoucher);
                if (v != null) v.DA_DUNG++;
            }

            db.SaveChanges();
            Session.Remove(CartSession);
            Session.Remove("MaVoucher");
            Session.Remove("SoTienGiam");

            return RedirectToAction("OrderSuccess");
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }

        // Đã sửa: Hàm _CartIcon
        [ChildActionOnly]
        public ActionResult _CartIcon()
        {
            List<CartItem> cart = GetCart();
            int totalItems = cart.Sum(item => item.SoLuong);
            return PartialView(totalItems);
        }

        // Đã sửa: Hàm Dispose (Quản lý CSDL)
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