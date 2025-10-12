using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN_SALE_LAPTOP.Models
{
    public class NhanVien
    {
        public string IDNhanVien { get; set; }
        public string FullNameNV { get; set; }
        public string ChucVuNV { get; set; }
        public string SDTNV { get; set; }
        public string EmailNV   { get; set; }
        public decimal LuongNV { get; set; }
        public DateTime DayStartWork { get; set; }

    }
}