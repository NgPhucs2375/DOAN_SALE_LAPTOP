using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN_SALE_LAPTOP.Models
{
    public class KhachHang
    {
        public string IDKhacHang { get; set; }
        public string FullNameKH {  get; set; }
        public string SDTKH {  get; set; }
        public string EmailKH { get; set; }
        public string DiaChiKH { get; set; }
        public DateTime DayBorn { get; set; }
        public string SexKH { get; set; }
        public string MatKhauKH { get; set; }

    }
}