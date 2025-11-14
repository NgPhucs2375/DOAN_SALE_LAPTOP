namespace WEB_SALE_LAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KHACHHANG")]
    public partial class KHACHHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KHACHHANG()
        {
            HOADONs = new HashSet<HOADON>();
            KHACHHANG_VOUCHER = new HashSet<KHACHHANG_VOUCHER>();
        }

        [Key]
        public int MAKH { get; set; }

        [Required]
        [StringLength(100)]
        public string HOTEN { get; set; }

        [StringLength(15)]
        public string SODT { get; set; }

        [StringLength(100)]
        public string EMAIL { get; set; }

        [StringLength(200)]
        public string DIACHI { get; set; }

        [StringLength(255)]
        public string MATKHAU { get; set; }

        public DateTime? NGAYTAO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOADON> HOADONs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KHACHHANG_VOUCHER> KHACHHANG_VOUCHER { get; set; }
    }
}
