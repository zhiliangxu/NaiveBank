using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class SafeBalancesController : ControllerBase
    {
        private readonly static ConcurrentDictionary<string, decimal> balances = new ConcurrentDictionary<string, decimal>();

        private readonly IAntiforgery antiforgery;

        static SafeBalancesController()
        {
            balances.TryAdd("alice", 100m);
            balances.TryAdd("eve", 0m);
        }

        public SafeBalancesController(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }

        [HttpGet]
        public ActionResult<ConcurrentDictionary<string, decimal>> Get()
        {
            return balances;
        }

        [HttpPost("transfer")]
        [CookieAuthorize]
        public ActionResult<decimal> Post([FromBody] TransferBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string from = (string)this.HttpContext.Items["User"];
            balances[from] -= request.Amount;
            balances[request.ToAccount] += request.Amount;
            return balances[from];
        }

        [HttpGet("antiforgerytoken")]
        [CookieAuthorize]
        public ActionResult<string> GetAntiforgeryToken()
        {
            return this.antiforgery.GetAndStoreTokens(this.HttpContext).RequestToken;
        }

        [HttpPost("validateantiforgerytoken")]
        [CookieAuthorize]
        [IgnoreAntiforgeryToken]
        public async Task<bool> ValidateAntiforgeryToken()
        {
           return await this.antiforgery.IsRequestValidAsync(this.HttpContext);

        }
    }
}
