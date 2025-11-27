using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;

namespace WEB_SALE_LAPTOP.Controllers
{
    public class VoucherManagementController : BaseAdminController
    {
        // Yêu cầu Quyền 2 (Bán hàng) hoặc 1 (Admin)
        public VoucherManagementController() : base(maQuyenCanCo: 2) { }

        // GET: Index
        public ActionResult Index()
        {
            return View(db.VOUCHERs.OrderByDescending(v => v.NGAYKETTHUC).ToList());
        }

        // GET: Create
        public ActionResult Create()
        {
            // Giá trị mặc định
            return View(new VOUCHER { NGAYBATDAU = DateTime.Now, NGAYKETTHUC = DateTime.Now.AddDays(30), SOLUONG_DUNG = 100, DA_DUNG = 0 });
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VOUCHER voucher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra mã trùng
                    if (db.VOUCHERs.Any(v => v.MAVOUCHER == voucher.MAVOUCHER))
                    {
                        ModelState.AddModelError("MAVOUCHER", "Mã Voucher này đã tồn tại!");
                        return View(voucher);
                    }

                    voucher.DA_DUNG = 0; // Luôn reset về 0 khi tạo mới
                    db.VOUCHERs.Add(voucher);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi khi tạo: " + ex.Message;
                }
            }
            return View(voucher);
        }

        // GET: Edit
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            VOUCHER voucher = db.VOUCHERs.Find(id);
            if (voucher == null) return HttpNotFound();
            return View(voucher);
        }

        // POST: Edit (Đã sửa lỗi cập nhật)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VOUCHER voucher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Cách cập nhật an toàn nhất: Tìm cái cũ -> Gán giá trị mới -> Lưu
                    var existingVoucher = db.VOUCHERs.Find(voucher.MAVOUCHER);
                    if (existingVoucher == null) return HttpNotFound();

                    existingVoucher.TEN_VOUCHER = voucher.TEN_VOUCHER;
                    existingVoucher.LOAI_GIAMGIA = voucher.LOAI_GIAMGIA;
                    existingVoucher.GIATRI = voucher.GIATRI;
                    existingVoucher.DONHANG_TOITHIEU = voucher.DONHANG_TOITHIEU;
                    existingVoucher.GIAM_TOIDA = voucher.GIAM_TOIDA;
                    existingVoucher.NGAYBATDAU = voucher.NGAYBATDAU;
                    existingVoucher.NGAYKETTHUC = voucher.NGAYKETTHUC;
                    existingVoucher.SOLUONG_DUNG = voucher.SOLUONG_DUNG;
                    // Không cập nhật DA_DUNG để tránh sai lệch số liệu

                    db.Entry(existingVoucher).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi cập nhật: " + ex.Message;
                }
            }
            return View(voucher);
        }

        // GET: Delete
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            VOUCHER voucher = db.VOUCHERs.Find(id);
            if (voucher == null) return HttpNotFound();
            return View(voucher);
        }

        // POST: Delete (Đã sửa lỗi xóa)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            VOUCHER voucher = db.VOUCHERs.Find(id);
            try
            {
                // Kiểm tra thủ công trước khi xóa để báo lỗi rõ ràng
                bool dangSuDung = db.HOADONs.Any(h => h.MAVOUCHER == id) || db.KHACHHANG_VOUCHER.Any(k => k.MAVOUCHER == id);

                if (dangSuDung)
                {
                    ViewBag.Error = "Không thể xóa Voucher này vì đã có lịch sử sử dụng trong Đơn hàng hoặc Ví khách hàng.";
                    return View(voucher); // Trả về View Delete để hiện lỗi
                }

                db.VOUCHERs.Remove(voucher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: Không thể xóa Voucher này. (Ràng buộc dữ liệu)";
                return View(voucher);
            }
        }
    }
}