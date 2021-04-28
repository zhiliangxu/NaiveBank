using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    public class BalancesController : Controller
    {
        private readonly static ConcurrentDictionary<string, decimal> balances = new ConcurrentDictionary<string, decimal>();

        static BalancesController()
        {
            balances.TryAdd("alice", 100);
            balances.TryAdd("bob", 20);
            balances.TryAdd("eve", 0);
        }

        [HttpGet()]
        public ActionResult<ConcurrentDictionary<string, decimal>> GetAllBalances()
        {
            return balances;
        }

        [HttpGet("me")]
        [CookieAuthorize]
        public ActionResult<string> GetMyBalance()
        {
            string user = (string)this.HttpContext.Items["User"];
            return $"{user}'s balance is: ${balances[user]}";
        }

        [HttpGet("transfer")]
        [CookieAuthorize]
        public ActionResult<string> TransferGet([FromQuery] string to, [FromQuery] decimal amount)
        {
            string from = (string)this.HttpContext.Items["User"];
            balances[from] -= amount;
            balances[to] += amount;
            return $"Successfully transferred ${amount} from {from} to {to}. Now {from}'s balance is ${balances[from]}";
        }
        
        [HttpPost("transfer")]
        [CookieAuthorize]
        public ActionResult<string> Transfer([FromBody] TransferBalanceRequest request)
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

        [HttpPost("transferA")]
        [CookieAuthorize]
        public ActionResult<string> Transfer(string toAccount, decimal amount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string from = (string)this.HttpContext.Items["User"];
            balances[from] -= amount;
            balances[toAccount] += amount;
            return $"Successfully transferred ${amount} from {from} to {toAccount}. Now {from}'s balance is ${balances[from]}";
        }
    }
}
