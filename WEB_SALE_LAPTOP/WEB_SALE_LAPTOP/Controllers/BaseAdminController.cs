using System;
using System.Collections.Generic; // <-- Thêm
using System.Linq; // <-- Thêm
using System.Web.Mvc;
using System.Web.Routing;
using WEB_SALE_LAPTOP.Models;

namespace WEB_SALE_LAPTOP.Controllers
{
    public abstract class BaseAdminController : Controller
    {
        protected QL_LAPTOP db = new QL_LAPTOP();

        // Biến này sẽ lưu quyền MÀ CONTROLLER CON YÊU CẦU
        private readonly int? _requiredQuyen;

        // Hàm khởi tạo (Constructor)
        // Controller con sẽ gọi hàm này để "nói" nó cần quyền gì
        public BaseAdminController(int? maQuyenCanCo = null)
        {
            _requiredQuyen = maQuyenCanCo;
        }

        // 2. Hàm "Bảo vệ" VÀ "Phân Quyền" (Đã "tiến hóa")
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //ViewBag.Layout = "~/Views/Shared/_AdminLayout.cshtml";

            // KIỂM TRA 1: Đã đăng nhập chưa?
            if (Session["AdminUser"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "AdminAccount", action = "Login" }
                    )
                );
                base.OnActionExecuting(filterContext);
                return; // Dừng lại ngay
            }

            // KIỂM TRA 2: Đã yêu cầu quyền cụ thể chưa?
            if (_requiredQuyen.HasValue)
            {
                // Lấy danh sách quyền của User từ Session
                var dsQuyen = Session["AdminQuyen"] as List<int>;

                // Kiểm tra xem User có quyền này không
                // (Giả sử 1 = Admin luôn có mọi quyền)
                if (dsQuyen == null || (!dsQuyen.Contains(_requiredQuyen.Value) && !dsQuyen.Contains(1)))
                {
                    // Nếu không có quyền, "đá" về trang "Cấm truy cập"
                    // (Bạn có thể tạo View này)
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new { controller = "AdminAccount", action = "AccessDenied" }
                        )
                    );
                }
            }

            base.OnActionExecuting(filterContext);
        }

        // 3. Cung cấp hàm "Dispose" chung (Giữ nguyên)
        protected override void Dispose(bool disposing)
        {
            if (disposing) { db.Dispose(); }
            base.Dispose(disposing);
        }
    }
}