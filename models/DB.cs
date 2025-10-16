using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DOAN_SALE_LAPTOP.Models
{
    public class DB
    {
        static string strcon = "Data Source=LAPTOP-CV633IP1;Initial Catalog=QL_Laptop;User ID=sa;Password=123;";
        SqlConnection con = new SqlConnection(strcon);
        public List<Laptop> dsLaptop = new List<Laptop>();
        public List<NhanVien> dsNhanVien = new List<NhanVien>();
        public List <KhachHang> dsKhachHang = new List<KhachHang> ();

        public DB()
        {
            Lap_ListLaptop();
            Lap_ListNhanVien();
            Lap_ListKhachHang();


        }

        public void Lap_ListLaptop()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * From LAPTOP", con);
            DataTable datatable = new DataTable();
            da.Fill(datatable);
            foreach (DataRow dr in datatable.Rows)
            {
                var t = new Laptop();
                t.IDLaptop = dr["MALAPTOP"].ToString();
                t.NameLaptop = dr["TENLAPTOP"].ToString();
                t.PriceLaptop = decimal.Parse(dr["GIA"].ToString());
                t.GraphLaptop = dr["CAUHINH"].ToString();
                t.IDLoai = dr["MALOAI"].ToString();
                t.IDCungCap = dr["MANCC"].ToString();
                t.HinhAnh = dr["HINHANH"].ToString();

                dsLaptop.Add(t);
            }


        }

        public void Lap_ListNhanVien()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * From NHANVIEN", con);
            DataTable datatable = new DataTable();
            da.Fill(datatable);
            foreach (DataRow dr in datatable.Rows)
            {
                var t = new NhanVien();
                t.IDNhanVien = dr["MANV"].ToString();
                t.FullNameNV = dr["HOTEN"].ToString();
                t.ChucVuNV = dr["CHUCVU"].ToString();
                t.SDTNV = dr["SODT"].ToString();
                t.EmailNV = dr["EMAIL"].ToString();
                t.LuongNV = decimal.Parse(dr["LUONG"].ToString());
                t.DayStartWork = DateTime.Parse(dr["NGAYVAOLAM"].ToString());


                dsNhanVien.Add(t);
            }


        }

        public void Lap_ListKhachHang()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * From KHACHHANG", con);
            DataTable datatable = new DataTable();
            da.Fill(datatable);
            foreach (DataRow dr in datatable.Rows)
            {
                var t = new KhachHang();
                t.IDKhacHang = dr["MAKH"].ToString();
                t.FullNameKH = dr["HOTEN"].ToString();
                t.SDTKH = dr["SODT"].ToString();
                t.EmailKH = dr["EMAIL"].ToString();
                t.DayBorn= DateTime.Parse(dr["NGAYSINH"].ToString());
                t.DiaChiKH = dr["DIACHI"].ToString();
                t.SexKH = dr["GIOITINH"].ToString();
                dsKhachHang.Add(t);
            }


        }

        // Phương thức tạo hóa đơn mới
        public string TaoHoaDon(HoaDon hoaDon)
        {
            try
            {
                con.Open();
                string maHoaDon = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");

                string query = @"INSERT INTO HOADON (MAHD, NGAYLAP, MAKH, MANV, TONGTIEN, TINHTRANG) 
                               VALUES (@MAHD, @NGAYLAP, @MAKH, @MANV, @TONGTIEN, @TINHTRANG)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MAHD", maHoaDon);
                cmd.Parameters.AddWithValue("@NGAYLAP", hoaDon.DayCreate);
                cmd.Parameters.AddWithValue("@MAKH", hoaDon.IDKH);
                cmd.Parameters.AddWithValue("@MANV", hoaDon.IDNV);
                cmd.Parameters.AddWithValue("@TONGTIEN", hoaDon.Total_Money);
                cmd.Parameters.AddWithValue("@TINHTRANG", hoaDon.State_TT);

                cmd.ExecuteNonQuery();
                return maHoaDon;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo hóa đơn: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Phương thức thêm chi tiết hóa đơn
        public void ThemChiTietHoaDon(CTHoaDon ctHoaDon)
        {
            try
            {
                con.Open();
                string query = @"INSERT INTO CTHOADON (MAHD, MALAPTOP, SOLUONG, DONGIA) 
                               VALUES (@MAHD, @MALAPTOP, @SOLUONG, @DONGIA)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MAHD", ctHoaDon.IDHoaDon);
                cmd.Parameters.AddWithValue("@MALAPTOP", ctHoaDon.IDLaptop);
                cmd.Parameters.AddWithValue("@SOLUONG", ctHoaDon.SLCT_HD);
                cmd.Parameters.AddWithValue("@DONGIA", ctHoaDon.DongGiaCT);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm chi tiết hóa đơn: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Phương thức lấy thông tin laptop theo ID
        public Laptop GetLaptopById(string id)
        {
            return dsLaptop.FirstOrDefault(l => l.IDLaptop == id);
        }

        // Phương thức lấy nhân viên ngẫu nhiên (tạm thời)
        public string GetRandomNhanVien()
        {
            var random = new Random();
            return dsNhanVien[random.Next(dsNhanVien.Count)].IDNhanVien;
        }

    }
}