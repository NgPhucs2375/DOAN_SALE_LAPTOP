namespace WEB_SALE_LAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("THANHTOAN")]
    public partial class THANHTOAN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MAHD { get; set; }

        [Required]
        [StringLength(50)]
        public string HINHTHUC { get; set; }

        public DateTime? NGAYTHANHTOAN { get; set; }

        public decimal? SOTIEN { get; set; }

        [StringLength(50)]
        public string TRANGTHAI { get; set; }

        public virtual HOADON HOADON { get; set; }
    }
}
