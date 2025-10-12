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
        static string strcon = "Data Source = MSI; database = QL_Laptop; User ID = sa;Password = 123456";
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
                t.MatKhauKH = dr["MATKHAU"].ToString();

                dsKhachHang.Add(t);
            }


        }

    }
}