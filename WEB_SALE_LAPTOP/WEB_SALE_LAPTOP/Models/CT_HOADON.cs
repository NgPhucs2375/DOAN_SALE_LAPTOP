namespace WEB_SALE_LAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CT_HOADON
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MAHD { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MALAPTOP { get; set; }

        public int? SOLUONG { get; set; }

        public decimal DONGIA { get; set; }

        public decimal? THANHTIEN { get; set; }

        public virtual HOADON HOADON { get; set; }

        public virtual LAPTOP LAPTOP { get; set; }
    }
}
