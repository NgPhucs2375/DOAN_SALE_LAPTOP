using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WEB_SALE_LAPTOP.Models
{
    public class CartItem
    {
        [Key]
        public int MaLaptop { get; set; }
        public string TenLaptop { get; set; }
        public string HinhAnh { get; set; }
        public decimal DonGia { get; set; }

        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1-100")]
        public int SoLuong { get; set; }

        // Thuộc tính tính toán
        public decimal ThanhTien
        {
            get { return SoLuong * DonGia; }
        }

        // Constructor
        public CartItem(int maLap, LAPTOP laptop, int soLuong = 1)
        {
            this.MaLaptop = maLap;
            this.TenLaptop = laptop.TENLAPTOP;
            this.HinhAnh = laptop.HINHANH0; // Lấy ảnh đại diện
            this.DonGia = laptop.GIA_BAN;
            this.SoLuong = soLuong;
        }
    }
}