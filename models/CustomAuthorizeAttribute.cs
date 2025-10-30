using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

// T?o custom Authorize Attribute
public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    private readonly string[] _roles;

    public CustomAuthorizeAttribute(params string[] roles)
    {
        _roles = roles;
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var isAuthorized = base.AuthorizeCore(httpContext);
        if (!isAuthorized)
        {
            return false;
        }

        var role = httpContext.Session["ROLE"]?.ToString();
        if (string.IsNullOrEmpty(role))
            return false;

        return _roles.Contains(role);
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectToRouteResult(
            new RouteValueDictionary(
                new
                {
                    controller = "Home",
                    action = "Login",
                    returnUrl = filterContext.HttpContext.Request.Url?.GetComponents(
                        UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                }));
    }
}