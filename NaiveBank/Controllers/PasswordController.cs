using Microsoft.AspNetCore.Mvc;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CookieAuthorize]
    public class PasswordController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> GetPassword()
        {
            return "v3RystR0N9p@55w0rD";
        }
    }
}
