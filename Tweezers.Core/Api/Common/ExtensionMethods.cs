using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tweezers.Api.Common
{
    public static class ExtensionMethods
    {
        public static ActionResult WithSecureCookie(this ActionResult result, HttpResponse response, string cookieKey, string cookieValue)
        {
            response.Cookies.Append(cookieKey, cookieValue, new CookieOptions()
            {
                Path = "/tweezers",
                IsEssential = true,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Domain = "NULL"
            });

            return result;
        }
    }
}
