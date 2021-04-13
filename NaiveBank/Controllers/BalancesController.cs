using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalancesController : ControllerBase
    {
        private readonly static ConcurrentDictionary<string, decimal> balances = new ConcurrentDictionary<string, decimal>();

        static BalancesController()
        {
            balances.TryAdd("alice", 100);
            balances.TryAdd("bob", 20);
            balances.TryAdd("eve", 0);
        }

        [HttpGet()]
        public ActionResult<ConcurrentDictionary<string, decimal>> GetAll()
        {
            return balances;
        }

        [HttpGet("me")]
        [CookieAuthorize]
        public ActionResult<string> Get()
        {
            string user = (string)this.HttpContext.Items["User"];
            return $"{user}'s balance is: ${balances[user]}";
        }

        [HttpGet("transfer")]
        [CookieAuthorize]
        public ActionResult<string> Get([FromQuery] string to, [FromQuery] decimal amount)
        {
            string from = (string)this.HttpContext.Items["User"];
            balances[from] -= amount;
            balances[to] += amount;
            return $"Successfully transferred ${amount} from {from} to {to}. Now {from}'s balance is ${balances[from]}";
        }
        
        [HttpPost("transfer")]
        [CookieAuthorize]
        public ActionResult<string> Post([FromBody] TransferBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string from = (string)this.HttpContext.Items["User"];
            balances[from] -= request.Amount;
            balances[request.ToAccount] += request.Amount;
            return $"Successfully transferred ${request.Amount} from {from} to {request.ToAccount}. Now {from}'s balance is ${balances[from]}";
        }

        [HttpPost("transferForm")]
        [CookieAuthorize]
        public ActionResult<string> PostForm([FromForm] TransferBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string from = (string)this.HttpContext.Items["User"];
            balances[from] -= request.Amount;
            balances[request.ToAccount] += request.Amount;
            return $"Successfully transferred ${request.Amount} from {from} to {request.ToAccount}. Now {from}'s balance is ${balances[from]}";
        }
    }
}
