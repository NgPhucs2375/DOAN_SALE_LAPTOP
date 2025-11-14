namespace WEB_SALE_LAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VOUCHER")]
    public partial class VOUCHER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VOUCHER()
        {
            HOADONs = new HashSet<HOADON>();
            KHACHHANG_VOUCHER = new HashSet<KHACHHANG_VOUCHER>();
        }

        [Key]
        [StringLength(20)]
        public string MAVOUCHER { get; set; }

        [StringLength(100)]
        public string TEN_VOUCHER { get; set; }

        [StringLength(10)]
        public string LOAI_GIAMGIA { get; set; }

        public decimal GIATRI { get; set; }

        public decimal? DONHANG_TOITHIEU { get; set; }

        public decimal? GIAM_TOIDA { get; set; }

        public DateTime NGAYBATDAU { get; set; }

        public DateTime NGAYKETTHUC { get; set; }

        public int? SOLUONG_DUNG { get; set; }

        public int? DA_DUNG { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOADON> HOADONs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KHACHHANG_VOUCHER> KHACHHANG_VOUCHER { get; set; }
    }
}
