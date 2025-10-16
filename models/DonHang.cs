using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN_SALE_LAPTOP.Models
{
    public class DonHang
    {
        public HoaDon HoaDon { get; set; }
        public List<CTHoaDon> ChiTietHoaDon { get; set; }

        public DonHang()
        {
            HoaDon = new HoaDon();
            ChiTietHoaDon = new List<CTHoaDon>();
        }
    }
}