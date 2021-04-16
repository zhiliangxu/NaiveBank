using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private string password;

        [HttpGet("login")]
        public string LogIn(string user = "default")
        {
            this.Response.Cookies.Append(
                "Auth",
                user,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.Now.AddDays(30),
                    SameSite = SameSiteMode.None //SameSiteMode.Strict, SameSiteMode.Lax
                });

            return $"Welcome {user}! You've logged in Naive Bank!";
        }

        [HttpGet("logout")]
        [CookieAuthorize]
        public string LogOut()
        {
            string user = (string)this.HttpContext.Items["User"];
            this.Response.Cookies.Delete("Auth");

            return $"Bye, {user}! You've logged out Naive Bank.";
        }

        [HttpGet("password")]
        [CookieAuthorize]
        public string GetPassword()
        {
            if (this.password == null)
            {
                using RandomNumberGenerator rng = RandomNumberGenerator.Create();
                byte[] bytes = new byte[32];
                rng.GetBytes(bytes);
                this.password = Convert.ToBase64String(bytes);
            }
            
            return this.password;
        }
    }
}
