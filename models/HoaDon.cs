using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN_SALE_LAPTOP.Models
{
    public class HoaDon
    {
        public string IDHoaDon { get; set; }
        public DateTime DayCreate { get; set; }
        public string IDKH {  get; set; }
        public string IDNV { get; set; }
        public decimal Total_Money { get; set; }
        public string State_TT { get; set; }

    }
}