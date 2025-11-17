using System.Web;
using System.Web.Optimization;

namespace WEB_SALE_LAPTOP
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //// (Giữ các bundle JS khác của bạn, ví dụ: bootstrap)
            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // --- CSS BUNDLES (ĐÃ TỐI ƯU) ---

            // Bundle CÔNG KHAI (Cho _Layout.cshtml)
            bundles.Add(new StyleBundle("~/Content/css/public").Include(
                      "~/Content/bootstrap.css", // (Tải bootstrap nếu bạn dùng)
                      "~/Content/css/site-public.css"));

            // Bundle ADMIN (Cho _AdminLayout.cshtml)
            bundles.Add(new StyleBundle("~/Content/css/admin").Include(
                      "~/Content/css/site-public.css", // Tải Bảng màu & Reset
                      "~/Content/css/site-admin.css")); // Tải style admin

            // Bundle CHECKOUT (Cho trang Giỏ hàng/Thanh toán)
            bundles.Add(new StyleBundle("~/Content/css/checkout").Include(
                      "~/Content/css/site-checkout.css"));

            // XÓA BUNDLE CŨ (Nếu có)
            // bundles.Add(new StyleBundle("~/Content/css").Include(
            //           "~/Content/bootstrap.css",
            //           "~/Content/css/style.css")); // <-- XÓA DÒNG NÀY
        }
    }
}