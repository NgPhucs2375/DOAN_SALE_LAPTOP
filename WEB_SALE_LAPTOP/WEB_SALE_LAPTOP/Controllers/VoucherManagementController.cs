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
    // Kế thừa "bộ não" Admin
    public class VoucherManagementController : BaseAdminController
    {
        // "TIẾN HÓA": Yêu cầu Quyền 2 (Bán hàng) hoặc 1 (Admin)
        // (Vì nó liên quan đến Bán hàng)
        public VoucherManagementController() : base(maQuyenCanCo: 2)
        {
            // Để trống
        }

        // GET: VoucherManagement
        public ActionResult Index()
        {
            return View(db.VOUCHERs.ToList());
        }

        // GET: VoucherManagement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VoucherManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAVOUCHER,TEN_VOUCHER,LOAI_GIAMGIA,GIATRI,DONHANG_TOITHIEU,GIAM_TOIDA,NGAYBATDAU,NGAYKETTHUC,SOLUONG_DUNG")] VOUCHER vOUCHER)
        {
            if (ModelState.IsValid)
            {
                // Gán (set) giá trị mặc định
                vOUCHER.DA_DUNG = 0;
                db.VOUCHERs.Add(vOUCHER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vOUCHER);
        }

        // GET: VoucherManagement/Edit/5
        public ActionResult Edit(string id) // <-- Khóa chính là string
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VOUCHER vOUCHER = db.VOUCHERs.Find(id);
            if (vOUCHER == null)
            {
                return HttpNotFound();
            }
            return View(vOUCHER);
        }

        // POST: VoucherManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAVOUCHER,TEN_VOUCHER,LOAI_GIAMGIA,GIATRI,DONHANG_TOITHIEU,GIAM_TOIDA,NGAYBATDAU,NGAYKETTHUC,SOLUONG_DUNG,DA_DUNG")] VOUCHER vOUCHER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vOUCHER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vOUCHER);
        }

        // GET: VoucherManagement/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VOUCHER vOUCHER = db.VOUCHERs.Find(id);
            if (vOUCHER == null)
            {
                return HttpNotFound();
            }
            return View(vOUCHER);
        }

        // POST: VoucherManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            VOUCHER vOUCHER = db.VOUCHERs.Find(id);
            try
            {
                db.VOUCHERs.Remove(vOUCHER);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Lỗi (nếu Voucher đã được dùng trong Hóa đơn)
                ViewBag.Error = "Không thể xóa Voucher này vì đã có Hóa đơn sử dụng!";
                return View(vOUCHER);
            }
            return RedirectToAction("Index");
        }
    }
}