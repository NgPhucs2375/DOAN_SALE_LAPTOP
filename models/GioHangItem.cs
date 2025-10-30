using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN_SALE_LAPTOP.Models
{
    public class GioHangItem
    {
        public string IDLaptop { get; set; }
        public string NameLaptop { get; set; }
        public decimal PriceLaptop { get; set; }
        public string GraphLaptop { get; set; }
        public string HinhAnh { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => PriceLaptop * Quantity;
    }
}