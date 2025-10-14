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
            using (SqlConnection con = new SqlConnection("Data Source = MSI; database = QL_Laptop; User ID = sa;Password = 123456"))
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
        [HttpPost]
        public ActionResult Login(string HoTen,string Email,string Password)
        {
            var KH = db.dsKhachHang.FirstOrDefault(kh=>kh.FullNameKH == HoTen && kh.EmailKH == Email && kh.MatKhauKH == Password );
            if(KH != null)
            {
                Session["KHACHHANG"] = KH; //save
                ViewBag.Message = "Đăng nhập thành công!";
                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewBag.Message = "Sai thông tin vui lòng kiểm tra lại thông tin!!!";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["KHACHHANG"] = null;
            return RedirectToAction("Login");
        }
        
    }
}