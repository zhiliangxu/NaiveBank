using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            this.Response.Cookies.Append(
                "Auth",
                "12344321",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.Now.AddDays(30),
                    SameSite = SameSiteMode.None //SameSiteMode.Strict, SameSiteMode.Lax
                });

            return "Welcome! We've logged in Naive Bank!";
        }
    }
}
