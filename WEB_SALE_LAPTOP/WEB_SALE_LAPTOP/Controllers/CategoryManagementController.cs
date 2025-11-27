using System;
using System.Linq;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;

namespace WEB_SALE_LAPTOP.Controllers
{
    // Kế thừa BaseAdminController để đảm bảo bảo mật (phải đăng nhập mới dùng được)
    public class CategoryManagementController : BaseAdminController
    {
        // Yêu cầu quyền Quản lý kho (3) hoặc Admin (1)
        public CategoryManagementController() : base(maQuyenCanCo: 3) { }

        // 1. VIEW CHÍNH (Giao diện)
        public ActionResult Index()
        {
            return View(); // Trả về file .cshtml chứa HTML/JS
        }

        // ==========================================
        // PHẦN API (TRẢ VỀ JSON CHO AJAX GỌI)
        // ==========================================

        // 2. API: Lấy danh sách (GET)
        [HttpGet]
        public JsonResult GetAll()
        {
            try
            {
                // Chọn các trường cần thiết để tránh lỗi vòng lặp (Circular Reference)
                var data = db.LOAI_LAPTOP
                             .Select(x => new {
                                 MaLoai = x.MALOAI,
                                 TenLoai = x.TENLOAI,
                                 SoLuongSP = x.LAPTOPs.Count() // Đếm số laptop thuộc loại này
                             })
                             .ToList();

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // 3. API: Thêm mới (POST)
        [HttpPost]
        public JsonResult Create(string tenLoai)
        {
            try
            {
                if (string.IsNullOrEmpty(tenLoai))
                    return Json(new { success = false, message = "Tên loại không được để trống!" });

                // Kiểm tra trùng tên
                if (db.LOAI_LAPTOP.Any(x => x.TENLOAI.ToLower() == tenLoai.ToLower()))
                    return Json(new { success = false, message = "Tên loại này đã tồn tại!" });

                var newItem = new LOAI_LAPTOP { TENLOAI = tenLoai };
                db.LOAI_LAPTOP.Add(newItem);
                db.SaveChanges();

                return Json(new { success = true, message = "Thêm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 4. API: Cập nhật (POST)
        [HttpPost]
        public JsonResult Update(int id, string tenLoai)
        {
            try
            {
                var item = db.LOAI_LAPTOP.Find(id);
                if (item == null)
                    return Json(new { success = false, message = "Không tìm thấy loại này!" });

                // Kiểm tra trùng tên (trừ chính nó ra)
                if (db.LOAI_LAPTOP.Any(x => x.TENLOAI.ToLower() == tenLoai.ToLower() && x.MALOAI != id))
                    return Json(new { success = false, message = "Tên loại đã được sử dụng!" });

                item.TENLOAI = tenLoai;
                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 5. API: Xóa (POST)
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var item = db.LOAI_LAPTOP.Find(id);
                if (item == null)
                    return Json(new { success = false, message = "Dữ liệu không tồn tại!" });

                if (item.LAPTOPs.Count > 0)
                {
                    return Json(new { success = false, message = $"Không thể xóa! Có {item.LAPTOPs.Count} sản phẩm đang thuộc loại này." });
                }

                db.LOAI_LAPTOP.Remove(item);
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}