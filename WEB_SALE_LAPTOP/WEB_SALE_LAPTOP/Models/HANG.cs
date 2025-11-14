namespace WEB_SALE_LAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HANG")]
    public partial class HANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HANG()
        {
            LAPTOPs = new HashSet<LAPTOP>();
        }

        [Key]
        public int MAHANG { get; set; }

        [Required]
        [StringLength(100)]
        public string TENHANG { get; set; }

        [StringLength(200)]
        public string DIACHI { get; set; }

        [StringLength(15)]
        public string SODT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LAPTOP> LAPTOPs { get; set; }
    }
}
