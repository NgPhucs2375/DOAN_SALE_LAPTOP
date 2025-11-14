using System.Web;
using System.Web.Mvc;

namespace WEB_SALE_LAPTOP
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
