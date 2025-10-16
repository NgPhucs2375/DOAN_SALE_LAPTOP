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
        public ActionResult register(string HoTen,string Email,string Password,string SDT)
        {
            using (SqlConnection con = new SqlConnection("Data Source = LAPTOP-CV633IP1; database = QL_Laptop; User ID = sa;Password = 123"))
            {
                string sql = "Insert into KHACHHANG (HOTEN,EMAIL,MATKHAU,SODT) VALUES (@hoten,@email,@matkhau,@sodt)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@hoten",HoTen);
                cmd.Parameters.AddWithValue("@email", Email);
                cmd.Parameters.AddWithValue("@matkhau", Password);
                cmd.Parameters.AddWithValue("@sodt", string.IsNullOrEmpty(SDT) ? (object)DBNull.Value : SDT);


                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }
            ViewBag.Message = "Đăng ký thành công! Hãy đăng nhập ngay.";
            return RedirectToAction("Login");        
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult Login(string HoTen,string Email,string Password)
        //{
        //    var KH = db.dsKhachHang.FirstOrDefault(kh=>kh.FullNameKH == HoTen && kh.EmailKH == Email && kh.MatKhauKH == Password );
        //    if(KH != null)
        //    {
        //        Session["KHACHHANG"] = KH; //save
        //        ViewBag.Message = "Đăng nhập thành công!";
        //        return RedirectToAction("Index","Home");
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Sai thông tin vui lòng kiểm tra lại thông tin!!!";
        //        return View();
        //    }
        //}


        //test !!! đăng nhập không cần mật khẩu vì chưa có thuộc tính mật khẩu trong data

        [HttpPost]
        public ActionResult Login(string Email)
        {
            // Chỉ cần Email để đăng nhập, không cần mật khẩu
            var KH = db.dsKhachHang.FirstOrDefault(kh => kh.EmailKH == Email);
            if (KH != null)
            {
                Session["KHACHHANG"] = KH; //save
                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Email không tồn tại! Vui lòng kiểm tra lại hoặc đăng ký tài khoản mới.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["KHACHHANG"] = null;
            return RedirectToAction("Login");
        }


        // Tính năng giỏ hàng
        // ========== GIỎ HÀNG ==========

        public ActionResult GioHang()
        {
            var gioHang = GetGioHang();
            return View(gioHang);
        }

        [HttpPost]
        public ActionResult ThemVaoGioHang(string id)
        {
            var laptop = db.dsLaptop.FirstOrDefault(l => l.IDLaptop == id);
            if (laptop != null)
            {
                var gioHang = GetGioHang();
                var existingItem = gioHang.Items.FirstOrDefault(i => i.IDLaptop == id);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
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
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult MuaNgay(string id)
        {
            var laptop = db.dsLaptop.FirstOrDefault(l => l.IDLaptop == id);
            if (laptop != null)
            {
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
                return RedirectToAction("ThanhToan");
            }

            return RedirectToAction("Index");
        }
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
                    itemTotal = item?.TotalPrice.ToString("N0"),
                    totalItems = gioHang.TotalItems
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
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

            return RedirectToAction("GioHang");
        }

        [HttpPost]
        public ActionResult XoaGioHang()
        {
            var gioHang = GetGioHang();
            gioHang.Items.Clear();
            SaveGioHang(gioHang);
            TempData["SuccessMessage"] = "Đã xóa toàn bộ giỏ hàng!";

            return RedirectToAction("GioHang");
        }



        // ========== THANH TOÁN ==========

        public ActionResult ThanhToan()
        {
            // Kiểm tra đăng nhập
            if (Session["KHACHHANG"] == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("Login");
            }

            var gioHang = GetGioHang();
            if (!gioHang.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("GioHang");
            }

            var khachHang = Session["KHACHHANG"] as KhachHang;
            ViewBag.KhachHang = khachHang;

            return View(gioHang);
        }

        [HttpPost]
        public ActionResult ThanhToan(string diaChiGiaoHang, string ghiChu)
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["KHACHHANG"] == null)
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                    return RedirectToAction("Login");
                }

                var gioHang = GetGioHang();
                if (gioHang == null || !gioHang.Items.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng trống!";
                    return RedirectToAction("GioHang");
                }

                var khachHang = Session["KHACHHANG"] as KhachHang;

                if (string.IsNullOrEmpty(diaChiGiaoHang))
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập địa chỉ giao hàng!";
                    return View(gioHang);
                }

                // Tạo hóa đơn
                var hoaDon = new HoaDon
                {
                    DayCreate = DateTime.Now,
                    IDKH = khachHang.IDKhacHang,
                    IDNV = "NV001", // Mã nhân viên mặc định
                    Total_Money = gioHang.TotalAmount,
                    State_TT = "Chờ xác nhận"
                };

                // Lưu hóa đơn vào database
                string maHoaDon = TaoHoaDon(hoaDon);

                // Lưu chi tiết hóa đơn
                foreach (var item in gioHang.Items)
                {
                    var ctHoaDon = new CTHoaDon
                    {
                        IDHoaDon = maHoaDon,
                        IDLaptop = item.IDLaptop,
                        SLCT_HD = item.Quantity,
                        DongGiaCT = item.PriceLaptop
                    };
                    ThemChiTietHoaDon(ctHoaDon);
                }

                // Xóa giỏ hàng sau khi đặt hàng thành công
                ClearGioHang();

                TempData["SuccessMessage"] = $"Đặt hàng thành công! Mã hóa đơn: {maHoaDon}";
                return RedirectToAction("DatHangThanhCong", new { id = maHoaDon });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi đặt hàng: " + ex.Message;
                var gioHang = GetGioHang();
                return View(gioHang);
            }
        }

        public ActionResult DatHangThanhCong(string id)
        {
            ViewBag.MaHoaDon = id;
            return View();
        }

        // ========== PHƯƠNG THỨC HỖ TRỢ ==========

        private string TaoHoaDon(HoaDon hoaDon)
        {
            using (SqlConnection con = new SqlConnection("Data Source = LAPTOP-CV633IP1; database = QL_Laptop; User ID = sa;Password = 123"))
            {
                string maHoaDon = "HD" + DateTime.Now.ToString("yy");

                string sql = @"INSERT INTO HOADON (MAHD, NGAYLAP, MAKH, MANV, TONGTIEN)
               VALUES (@mahd, @ngaylap, @makh, @manv, @tongtien)";

               

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@mahd", maHoaDon);
                cmd.Parameters.AddWithValue("@ngaylap", hoaDon.DayCreate);
                cmd.Parameters.AddWithValue("@makh", hoaDon.IDKH);
                cmd.Parameters.AddWithValue("@manv", hoaDon.IDNV);
                cmd.Parameters.AddWithValue("@tongtien", hoaDon.Total_Money);
   
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return maHoaDon;
            }
        }

        private void ThemChiTietHoaDon(CTHoaDon ctHoaDon)
        {
            using (SqlConnection con = new SqlConnection("Data Source = LAPTOP-CV633IP1; database = QL_Laptop; User ID = sa;Password = 123"))
            {
                string sql = @"INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA) 
                             VALUES (@mahd, @malaptop, @soluong, @dongia)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@mahd", ctHoaDon.IDHoaDon);
                cmd.Parameters.AddWithValue("@malaptop", ctHoaDon.IDLaptop);
                cmd.Parameters.AddWithValue("@soluong", ctHoaDon.SLCT_HD);
                cmd.Parameters.AddWithValue("@dongia", ctHoaDon.DongGiaCT);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

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


    }
}