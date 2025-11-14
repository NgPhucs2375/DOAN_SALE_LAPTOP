namespace WEB_SALE_LAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KHACHHANG_VOUCHER
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MAKH { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string MAVOUCHER { get; set; }

        public DateTime? NGAYDUNG { get; set; }

        public int? MAHD { get; set; }

        public virtual HOADON HOADON { get; set; }

        public virtual KHACHHANG KHACHHANG { get; set; }

        public virtual VOUCHER VOUCHER { get; set; }
    }
}
