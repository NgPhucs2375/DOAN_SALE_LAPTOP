using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN_SALE_LAPTOP.Models
{
    public class Laptop
    {
        public string IDLaptop { get; set; }
        public string NameLaptop {  set; get; }
        public decimal PriceLaptop { set; get; }
        public string GraphLaptop { set; get; }
        public string IDLoai { set; get; }
        public string IDCungCap {  set; get; }

    }
}