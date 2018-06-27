using Microsoft.AspNetCore.Mvc;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            this.Response.Cookies.Delete("Auth");

            return "Bye! We've logged out Naive Bank!";
        }
    }
}