using System.Collections.Generic;
using System.Web.Mvc;
using PagedList; 

namespace WEB_SALE_LAPTOP.Models
{
    public class ProductManagementViewModel
    {
        // 1. Thống kê (Statistics)
        public int TongSoSanPham { get; set; }
        public int SanPhamDangBan { get; set; }
        public int SanPhamDaAn { get; set; }


        public IPagedList<LAPTOP> Products { get; set; }

        public SelectList Brands { get; set; } // (Lọc Hãng)
        public SelectList Categories { get; set; } // (Lọc Loại)

        public int? CurrentBrandId { get; set; }
        public int? CurrentCategoryId { get; set; }
        public string CurrentSearch { get; set; }
    }
}