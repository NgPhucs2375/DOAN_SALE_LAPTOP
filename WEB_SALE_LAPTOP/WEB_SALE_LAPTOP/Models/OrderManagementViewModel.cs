using System.Collections.Generic;

namespace WEB_SALE_LAPTOP.Models
{
    // "Bộ não" mới, chứa MỌI THỨ mà trang Admin cần
    public class OrderManagementViewModel
    {
        // 1. Thống kê (Statistics)
        public int TongDonHang { get; set; }
        public int DonChoXuLy { get; set; }
        public int DonDangGiao { get; set; }
        public decimal TongDoanhThu { get; set; } // (Chỉ tính đơn "Hoàn thành")

        // 2. Dữ liệu Bảng (Table Data)
        public List<HOADON> Hoadons { get; set; }

        // 3. Trạng thái (để biết đang lọc cái gì)
        public string CurrentFilter { get; set; }
    }
}