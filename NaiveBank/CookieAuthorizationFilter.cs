using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NaiveBank
{
    public class CookieAuthorizeAttribute : TypeFilterAttribute
    {
        public CookieAuthorizeAttribute() : base(typeof(CookieAuthorizationFilter))
        {
        }
    }

    public class CookieAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string authCookie = context.HttpContext.Request.Cookies["Auth"];
            if (authCookie != "12344321")
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
