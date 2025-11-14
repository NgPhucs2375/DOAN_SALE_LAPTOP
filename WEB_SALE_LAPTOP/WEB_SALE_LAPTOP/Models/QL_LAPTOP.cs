using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace WEB_SALE_LAPTOP.Models
{
    public partial class QL_LAPTOP : DbContext
    {
        public QL_LAPTOP()
            : base("name=QL_LAPTOP")
        {
        }

        public virtual DbSet<CT_HOADON> CT_HOADON { get; set; }
        public virtual DbSet<HANG> HANGs { get; set; }
        public virtual DbSet<HOADON> HOADONs { get; set; }
        public virtual DbSet<KHACHHANG> KHACHHANGs { get; set; }
        public virtual DbSet<KHACHHANG_VOUCHER> KHACHHANG_VOUCHER { get; set; }
        public virtual DbSet<LAPTOP> LAPTOPs { get; set; }
        public virtual DbSet<LOAI_LAPTOP> LOAI_LAPTOP { get; set; }
        public virtual DbSet<NHANVIEN> NHANVIENs { get; set; }
        public virtual DbSet<PHANQUYEN> PHANQUYENs { get; set; }
        public virtual DbSet<THANHTOAN> THANHTOANs { get; set; }
        public virtual DbSet<VOUCHER> VOUCHERs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CT_HOADON>()
                .Property(e => e.DONGIA)
                .HasPrecision(15, 2);

            modelBuilder.Entity<CT_HOADON>()
                .Property(e => e.THANHTIEN)
                .HasPrecision(15, 2);

            modelBuilder.Entity<HANG>()
                .Property(e => e.SODT)
                .IsUnicode(false);

            modelBuilder.Entity<HANG>()
                .HasMany(e => e.LAPTOPs)
                .WithRequired(e => e.HANG)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HOADON>()
                .Property(e => e.TONGTIEN_HANG)
                .HasPrecision(15, 2);

            modelBuilder.Entity<HOADON>()
                .Property(e => e.MAVOUCHER)
                .IsUnicode(false);

            modelBuilder.Entity<HOADON>()
                .Property(e => e.SOTIEN_GIAM_VOUCHER)
                .HasPrecision(15, 2);

            modelBuilder.Entity<HOADON>()
                .Property(e => e.TONG_THANHTOAN)
                .HasPrecision(15, 2);

            modelBuilder.Entity<HOADON>()
                .Property(e => e.SDT_GIAO)
                .IsUnicode(false);

            modelBuilder.Entity<HOADON>()
                .HasMany(e => e.CT_HOADON)
                .WithRequired(e => e.HOADON)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HOADON>()
                .HasOptional(e => e.THANHTOAN)
                .WithRequired(e => e.HOADON);

            modelBuilder.Entity<KHACHHANG>()
                .Property(e => e.SODT)
                .IsUnicode(false);

            modelBuilder.Entity<KHACHHANG>()
                .Property(e => e.MATKHAU)
                .IsUnicode(false);

            modelBuilder.Entity<KHACHHANG>()
                .HasMany(e => e.KHACHHANG_VOUCHER)
                .WithRequired(e => e.KHACHHANG)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KHACHHANG_VOUCHER>()
                .Property(e => e.MAVOUCHER)
                .IsUnicode(false);

            modelBuilder.Entity<LAPTOP>()
                .Property(e => e.GIA_GOC)
                .HasPrecision(15, 2);

            modelBuilder.Entity<LAPTOP>()
                .Property(e => e.GIA_BAN)
                .HasPrecision(15, 2);

            modelBuilder.Entity<LAPTOP>()
                .HasMany(e => e.CT_HOADON)
                .WithRequired(e => e.LAPTOP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LOAI_LAPTOP>()
                .HasMany(e => e.LAPTOPs)
                .WithRequired(e => e.LOAI_LAPTOP)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NHANVIEN>()
                .Property(e => e.MATKHAU)
                .IsUnicode(false);

            modelBuilder.Entity<NHANVIEN>()
                .HasMany(e => e.PHANQUYENs)
                .WithMany(e => e.NHANVIENs)
                .Map(m => m.ToTable("CAPQUYEN").MapLeftKey("MANV").MapRightKey("MAQUYEN"));

            modelBuilder.Entity<THANHTOAN>()
                .Property(e => e.SOTIEN)
                .HasPrecision(15, 2);

            modelBuilder.Entity<VOUCHER>()
                .Property(e => e.MAVOUCHER)
                .IsUnicode(false);

            modelBuilder.Entity<VOUCHER>()
                .Property(e => e.LOAI_GIAMGIA)
                .IsUnicode(false);

            modelBuilder.Entity<VOUCHER>()
                .Property(e => e.GIATRI)
                .HasPrecision(15, 2);

            modelBuilder.Entity<VOUCHER>()
                .Property(e => e.DONHANG_TOITHIEU)
                .HasPrecision(15, 2);

            modelBuilder.Entity<VOUCHER>()
                .Property(e => e.GIAM_TOIDA)
                .HasPrecision(15, 2);

            modelBuilder.Entity<VOUCHER>()
                .HasMany(e => e.KHACHHANG_VOUCHER)
                .WithRequired(e => e.VOUCHER)
                .WillCascadeOnDelete(false);
        }

        public void InsertOnSubmit(LAPTOP laptop)
        {
            LAPTOPs.Add(laptop);
        }
        public void InsertOnSubmit(KHACHHANG khachhang)
        {
            KHACHHANGs.Add(khachhang);
        }
        public void InsertOnSubmit(NHANVIEN nhanvien)
        {
            NHANVIENs.Add(nhanvien);
        }
        public void InsertOnSubmit(HOADON hoandon)
        {
            HOADONs.Add(hoandon);
        }
        public void UpdateOnSubmit(LAPTOP laptop)
        {
            LAPTOPs.AddOrUpdate(laptop);
        }
        public void DeleteOnSubmit(LAPTOP laptop)
        {
            LAPTOPs.Remove(laptop);
        }
        public void SubmitChanges()
        {
            SaveChanges();
        }

    }
}
