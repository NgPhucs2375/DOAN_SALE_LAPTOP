using DOAN_SALE_LAPTOP.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DOAN_SALE_LAPTOP.Controllers
{
    public class HomeController : Controller
    {
        DB db = new DB();

        // Ensure in-memory lists are populated for lookups (laptop, customer, employee)
        public HomeController()
        {
            try
            {
                db.Lap_ListLaptop();
                db.Lap_ListKhachHang();
                db.Lap_ListNhanVien();
            }
            catch
            {
                // optionally log
            }
        }

        // Actions cho mọi người
        public ActionResult Index()
        {
            var laptop = db.dsLaptop;
            return View(db.dsLaptop);
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var laptop = db.dsLaptop.FirstOrDefault(l => l.IDLaptop == id);
            if (laptop == null)
                return HttpNotFound();

            // Lấy danh sách sản phẩm liên quan theo cùng loại (IDLoai), loại trừ sản phẩm hiện tại, giới hạn 4
            try
            {
                var related = db.dsLaptop
                    .Where(l => l.IDLoai == laptop.IDLoai && l.IDLaptop != id)
                    .Take(4)
                    .ToList();
                ViewBag.RelatedProducts = related;
            }
            catch
            {
                ViewBag.RelatedProducts = new List<Laptop>();
            }

            return View(laptop);
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

        [HttpGet]
        public ActionResult register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult register(string HoTen, string Email, string Password, string SDT)
        {
            try
            {
                // Validate đầu vào
                if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ thông tin bắt buộc";
                    return View();
                }

                using (SqlConnection con = new SqlConnection("Data Source = MSI; database = QL_LAPTOP; User ID = sa;Password = 123456"))
                {
                    con.Open();

                    // Kiểm tra email đã tồn tại chưa
                    string checkEmail = "SELECT COUNT(*) FROM KHACHHANG WHERE EMAIL = @email";
                    using (SqlCommand cmdCheck = new SqlCommand(checkEmail, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@email", Email.Trim());
                        int count = (int)cmdCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            ViewBag.Error = "Email này đã được đăng ký!";
                            return View();
                        }
                    }

                    // Insert khách hàng mới
                    string sql = @"INSERT INTO KHACHHANG (HOTEN, EMAIL, MATKHAU, SODT) 
                                   VALUES (@hoten, @email, @matkhau, @sodt)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@hoten", HoTen.Trim());
                        cmd.Parameters.AddWithValue("@email", Email.Trim());
                        cmd.Parameters.AddWithValue("@matkhau", Password.Trim());
                        cmd.Parameters.AddWithValue("@sodt", string.IsNullOrWhiteSpace(SDT) ? (object)DBNull.Value : SDT.Trim());

                        cmd.ExecuteNonQuery();
                    }

                    // Refresh danh sách khách hàng trong DB class
                    db.Lap_ListKhachHang();

                    TempData["SuccessMessage"] = "Đăng ký thành công! Hãy đăng nhập ngay.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Có lỗi xảy ra khi đăng ký. Vui lòng thử lại sau.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            try
            {
                Email = Email?.Trim();
                Password = Password?.Trim();

                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu";
                    return View();
                }

                // Đảm bảo dữ liệu mới nhất
                db.Lap_ListKhachHang();
                db.Lap_ListNhanVien();

                // Kiểm tra khách hàng
                var khachHang = db.dsKhachHang.FirstOrDefault(kh =>
                    kh.EmailKH != null && kh.EmailKH.Equals(Email, StringComparison.OrdinalIgnoreCase));

                if (khachHang != null && string.Equals(khachHang.MatKhauKH, Password, StringComparison.Ordinal))
                {
                    Session["USER"] = khachHang;
                    Session["ROLE"] = "CUSTOMER";
                    Session["KHACHHANG"] = khachHang;

                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                }

                // Kiểm tra nhân viên
                var nhanVien = db.dsNhanVien.FirstOrDefault(nv =>
                    nv.EmailNV != null && nv.EmailNV.Equals(Email, StringComparison.OrdinalIgnoreCase));

                if (nhanVien != null && string.Equals(nhanVien.MatKhauNV, Password, StringComparison.Ordinal))
                {
                    Session["USER"] = nhanVien;
                    Session["ROLE"] = nhanVien.ChucVuNV == "Admin" ? "ADMIN" : "STAFF";

                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Dashboard", "Admin");
                }

                ViewBag.Error = "Email hoặc mật khẩu không chính xác";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại sau.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

       // Plan:
        // - Remove call to GetGioHang() since cart logic moved to GioHangController.
        // - Redirect to GioHangController.Index to handle cart retrieval and view rendering.
        // - Keep authorization attribute unchanged.

        [CustomAuthorize("CUSTOMER")]
        public ActionResult GioHang()
        {
            return RedirectToAction("Index", "GioHang");
        }

        [HttpPost]
        public ActionResult ThemVaoGio(string id)
        {
            return RedirectToAction("ThemVaoGio", "GioHang", new { id });
        }


        // Chuyển sang controller GioHangController để dễ quản lý hơn

        //[HttpPost]
        //public ActionResult ThemVaoGioHang(string id)
        //{
        //    var laptop = db.dsLaptop.FirstOrDefault(l => l.IDLaptop == id);
        //    if (laptop != null)
        //    {
        //        var gioHang = GetGioHang();
        //        var existingItem = gioHang.Items.FirstOrDefault(i => i.IDLaptop == id);

        //        if (existingItem != null)
        //        {
        //            existingItem.Quantity++;
        //        }
        //        else
        //        {
        //            gioHang.Items.Add(new GioHangItem
        //            {
        //                IDLaptop = laptop.IDLaptop,
        //                NameLaptop = laptop.NameLaptop,
        //                PriceLaptop = laptop.PriceLaptop,
        //                GraphLaptop = laptop.GraphLaptop,
        //                HinhAnh = laptop.HinhAnh,
        //                Quantity = 1
        //            });
        //        }

        //        SaveGioHang(gioHang);
        //        TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
        //    }

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public ActionResult MuaNgay(string id)
        //{
        //    var laptop = db.dsLaptop.FirstOrDefault(l => l.IDLaptop == id);
        //    if (laptop != null)
        //    {
        //        var gioHang = GetGioHang();
        //        gioHang.Items.Clear();

        //        gioHang.Items.Add(new GioHangItem
        //        {
        //            IDLaptop = laptop.IDLaptop,
        //            NameLaptop = laptop.NameLaptop,
        //            PriceLaptop = laptop.PriceLaptop,
        //            GraphLaptop = laptop.GraphLaptop,
        //            HinhAnh = laptop.HinhAnh,
        //            Quantity = 1
        //        });

        //        SaveGioHang(gioHang);
        //        return RedirectToAction("ThanhToan");
        //    }

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public ActionResult CapNhatSoLuong(string id, int quantity)
        //{
        //    try
        //    {
        //        var gioHang = GetGioHang();
        //        var item = gioHang.Items.FirstOrDefault(i => i.IDLaptop == id);

        //        if (item != null)
        //        {
        //            if (quantity <= 0)
        //            {
        //                gioHang.Items.Remove(item);
        //            }
        //            else
        //            {
        //                item.Quantity = quantity;
        //            }
        //            SaveGioHang(gioHang);
        //        }

        //        return Json(new
        //        {
        //            success = true,
        //            totalAmount = gioHang.TotalAmount.ToString("N0"),
        //            itemTotal = item?.TotalPrice.ToString("N0"),
        //            totalItems = gioHang.TotalItems
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public ActionResult XoaKhoiGioHang(string id)
        //{
        //    var gioHang = GetGioHang();
        //    var item = gioHang.Items.FirstOrDefault(i => i.IDLaptop == id);

        //    if (item != null)
        //    {
        //        gioHang.Items.Remove(item);
        //        SaveGioHang(gioHang);
        //        TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
        //    }

        //    return RedirectToAction("GioHang");
        //}

        //[HttpPost]
        //public ActionResult XoaGioHang()
        //{
        //    var gioHang = GetGioHang();
        //    gioHang.Items.Clear();
        //    SaveGioHang(gioHang);
        //    TempData["SuccessMessage"] = "Đã xóa toàn bộ giỏ hàng!";

        //    return RedirectToAction("GioHang");
        //}

        //[CustomAuthorize("CUSTOMER")]
        //public ActionResult ThanhToan()
        //{
        //    if (Session["KHACHHANG"] == null)
        //    {
        //        TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
        //        return RedirectToAction("Login");
        //    }

        //    var gioHang = GetGioHang();
        //    if (!gioHang.Items.Any())
        //    {
        //        TempData["ErrorMessage"] = "Giỏ hàng trống!";
        //        return RedirectToAction("GioHang");
        //    }

        //    var khachHang = Session["KHACHHANG"] as KhachHang;
        //    ViewBag.KhachHang = khachHang;

        //    return View(gioHang);
        //}

        //[HttpPost]
        //public ActionResult ThanhToan(string diaChiGiaoHang, string ghiChu)
        //{
        //    try
        //    {
        //        if (Session["KHACHHANG"] == null)
        //        {
        //            TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
        //            return RedirectToAction("Login");
        //        }

        //        var gioHang = GetGioHang();
        //        if (gioHang == null || !gioHang.Items.Any())
        //        {
        //            TempData["ErrorMessage"] = "Giỏ hàng trống!";
        //            return RedirectToAction("GioHang");
        //        }

        //        var khachHang = Session["KHACHHANG"] as KhachHang;

        //        if (string.IsNullOrEmpty(diaChiGiaoHang))
        //        {
        //            TempData["ErrorMessage"] = "Vui lòng nhập địa chỉ giao hàng!";
        //            return View(gioHang);
        //        }

        //        var hoaDon = new HoaDon
        //        {
        //            DayCreate = DateTime.Now,
        //            IDKH = khachHang.IDKhacHang,
        //            IDNV = "NV001",
        //            Total_Money = gioHang.TotalAmount,
        //            State_TT = "Chờ xác nhận"
        //        };

        //        string maHoaDon = TaoHoaDon(hoaDon);

        //        foreach (var item in gioHang.Items)
        //        {
        //            var ctHoaDon = new CTHoaDon
        //            {
        //                IDHoaDon = maHoaDon,
        //                IDLaptop = item.IDLaptop,
        //                SLCT_HD = item.Quantity,
        //                DongGiaCT = item.PriceLaptop
        //            };
        //            ThemChiTietHoaDon(ctHoaDon);
        //        }

        //        ClearGioHang();

        //        TempData["SuccessMessage"] = $"Đặt hàng thành công! Mã hóa đơn: {maHoaDon}";
        //        return RedirectToAction("DatHangThanhCong", new { id = maHoaDon });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Lỗi khi đặt hàng: " + ex.Message;
        //        var gioHang = GetGioHang();
        //        return View(gioHang);
        //    }
        //}

        //public ActionResult DatHangThanhCong(string id)
        //{
        //    ViewBag.MaHoaDon = id;
        //    return View();
        //}

        //private string TaoHoaDon(HoaDon hoaDon)
        //{
        //    using (SqlConnection con = new SqlConnection("Data Source = MSI; database = QL_LAPTOP; User ID = sa;Password = 123456"))
        //    {
        //        string maHoaDon = "HD" + DateTime.Now.ToString("yy");

        //        string sql = @"INSERT INTO HOADON (MAHD, NGAYLAP, MAKH, MANV, TONGTIEN)
        //                       VALUES (@mahd, @ngaylap, @makh, @manv, @tongtien)";

        //        SqlCommand cmd = new SqlCommand(sql, con);
        //        cmd.Parameters.AddWithValue("@mahd", maHoaDon);
        //        cmd.Parameters.AddWithValue("@ngaylap", hoaDon.DayCreate);
        //        cmd.Parameters.AddWithValue("@makh", hoaDon.IDKH);
        //        cmd.Parameters.AddWithValue("@manv", hoaDon.IDNV);
        //        cmd.Parameters.AddWithValue("@tongtien", hoaDon.Total_Money);

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();

        //        return maHoaDon;
        //    }
        //}

        //private void ThemChiTietHoaDon(CTHoaDon ctHoaDon)
        //{
        //    using (SqlConnection con = new SqlConnection("Data Source = MSI; database = QL_LAPTOP; User ID = sa;Password = 123456"))
        //    {
        //        string sql = @"INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA) 
        //                       VALUES (@mahd, @malaptop, @soluong, @dongia)";

        //        SqlCommand cmd = new SqlCommand(sql, con);
        //        cmd.Parameters.AddWithValue("@mahd", ctHoaDon.IDHoaDon);
        //        cmd.Parameters.AddWithValue("@malaptop", ctHoaDon.IDLaptop);
        //        cmd.Parameters.AddWithValue("@soluong", ctHoaDon.SLCT_HD);
        //        cmd.Parameters.AddWithValue("@dongia", ctHoaDon.DongGiaCT);

        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();
        //    }
        //}

        //private GioHang GetGioHang()
        //{
        //    var gioHang = Session["GioHang"] as GioHang;
        //    if (gioHang == null)
        //    {
        //        gioHang = new GioHang();
        //        Session["GioHang"] = gioHang;
        //    }
        //    return gioHang;
        //}

        //private void SaveGioHang(GioHang gioHang)
        //{
        //    Session["GioHang"] = gioHang;
        //}

        //private void ClearGioHang()
        //{
        //    Session["GioHang"] = new GioHang();
        //}
    }
}