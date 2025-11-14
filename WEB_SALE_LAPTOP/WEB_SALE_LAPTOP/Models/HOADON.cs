namespace WEB_SALE_LAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HOADON")]
    public partial class HOADON
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HOADON()
        {
            CT_HOADON = new HashSet<CT_HOADON>();
            KHACHHANG_VOUCHER = new HashSet<KHACHHANG_VOUCHER>();
        }

        [Key]
        public int MAHD { get; set; }

        public DateTime? NGAYLAP { get; set; }

        public int? MAKH { get; set; }

        public int? MANV { get; set; }

        public decimal? TONGTIEN_HANG { get; set; }

        [StringLength(20)]
        public string MAVOUCHER { get; set; }

        public decimal? SOTIEN_GIAM_VOUCHER { get; set; }

        public decimal? TONG_THANHTOAN { get; set; }

        [StringLength(50)]
        public string TRANGTHAI { get; set; }

        [StringLength(200)]
        public string DIACHI_GIAO { get; set; }

        [StringLength(15)]
        public string SDT_GIAO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CT_HOADON> CT_HOADON { get; set; }

        public virtual KHACHHANG KHACHHANG { get; set; }

        public virtual NHANVIEN NHANVIEN { get; set; }

        public virtual VOUCHER VOUCHER { get; set; }

        public virtual THANHTOAN THANHTOAN { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KHACHHANG_VOUCHER> KHACHHANG_VOUCHER { get; set; }
    }
}
