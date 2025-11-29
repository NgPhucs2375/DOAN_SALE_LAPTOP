using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB_SALE_LAPTOP.Models
{
    public class CustomerProfileViewModel
    {
        // 1. Thông tin để "Sửa" (Edit)
        public KHACHHANG CustomerInfo { get; set; }

        // 2. Nội dung "mới" (New Content) để lấp đầy trang
        public List<HOADON> OrderHistory { get; set; }
    }
}