using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN_SALE_LAPTOP.Models
{
    public class GioHang
    {
        public List<GioHangItem> Items { get; set; }

        public GioHang()
        {
            Items = new List<GioHangItem>();
        }

        public decimal TotalAmount => Items?.Sum(x => x.TotalPrice) ?? 0;
        public int TotalItems => Items?.Sum(x => x.Quantity) ?? 0;


    }
}