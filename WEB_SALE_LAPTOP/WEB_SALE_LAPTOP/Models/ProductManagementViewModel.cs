using System.Collections.Generic;
using System.Web.Mvc;
using PagedList; // <-- "Bộ não" (brain) Phân trang (Pagination) chúng ta vừa cài

namespace WEB_SALE_LAPTOP.Models
{
    // "Bộ não" (brain) mới cho trang Quản lý Sản phẩm
    public class ProductManagementViewModel
    {
        // 1. Thống kê (Statistics)
        public int TongSoSanPham { get; set; }
        public int SanPhamDangBan { get; set; }
        public int SanPhamDaAn { get; set; }

        // 2. Dữ liệu Bảng (Table Data) "Đã Phân trang" (Paginated)
        // Đây là "danh sách" (list) "tiến hóa" (evolved)
        public IPagedList<LAPTOP> Products { get; set; }

        // 3. Dữ liệu cho Bộ lọc (Filters)
        public SelectList Brands { get; set; } // (Lọc Hãng)
        public SelectList Categories { get; set; } // (Lọc Loại)

        // 4. Ghi nhớ (Remember) lựa chọn lọc
        public int? CurrentBrandId { get; set; }
        public int? CurrentCategoryId { get; set; }
        public string CurrentSearch { get; set; }
    }
}