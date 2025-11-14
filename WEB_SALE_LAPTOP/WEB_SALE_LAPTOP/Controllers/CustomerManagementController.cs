using System.Linq;
using System.Web.Mvc;
using WEB_SALE_LAPTOP.Models;

namespace WEB_SALE_LAPTOP.Controllers
{
    // Kế thừa "bộ não" Admin
    public class CustomerManagementController : BaseAdminController
    {
        // "TIẾN HÓA": Yêu cầu Quyền 1 (Admin cao nhất)
        // (Vì đây là thông tin nhạy cảm của khách hàng)
        public CustomerManagementController() : base(maQuyenCanCo: 1)
        {
            // Để trống
        }

        // GET: CustomerManagement (Danh sách Khách hàng)
        public ActionResult Index()
        {
            var khachHangs = db.KHACHHANGs
                                .OrderByDescending(kh => kh.NGAYTAO)
                                .ToList();

            // View này sẽ tự động dùng "_AdminLayout.cshtml"
            return View(khachHangs);
        }
    }
}