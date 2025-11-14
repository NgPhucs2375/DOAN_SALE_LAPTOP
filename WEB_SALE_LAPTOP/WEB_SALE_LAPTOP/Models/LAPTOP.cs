namespace WEB_SALE_LAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LAPTOP")]
    public partial class LAPTOP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LAPTOP()
        {
            CT_HOADON = new HashSet<CT_HOADON>();
        }

        [Key]
        public int MALAPTOP { get; set; }

        [Required]
        [StringLength(200)]
        public string TENLAPTOP { get; set; }

        public string MOTA { get; set; }

        [StringLength(200)]
        public string CAUHINH { get; set; }

        public decimal GIA_GOC { get; set; }

        public decimal GIA_BAN { get; set; }

        public int? SOLUONG_TON { get; set; }

        [StringLength(255)]
        public string HINHANH0 { get; set; }

        [StringLength(255)]
        public string HINHANH1 { get; set; }

        [StringLength(255)]
        public string HINHANH2 { get; set; }

        [StringLength(255)]
        public string HINHANH3 { get; set; }

        public int MALOAI { get; set; }

        public int MAHANG { get; set; }

        public bool? TRANGTHAI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CT_HOADON> CT_HOADON { get; set; }

        public virtual HANG HANG { get; set; }

        public virtual LOAI_LAPTOP LOAI_LAPTOP { get; set; }
    }
}
