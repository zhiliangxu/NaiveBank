using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalancesController : ControllerBase
    {
        private readonly static ConcurrentDictionary<int, decimal> balances = new ConcurrentDictionary<int, decimal>();

        static BalancesController()
        {
            balances.TryAdd(1, 100m);
            balances.TryAdd(2, 0m);
        }

        [HttpGet]
        public ActionResult<ConcurrentDictionary<int, decimal>> Get()
        {
            return balances;
        }

        [HttpGet("{id}")]
        [CookieAuthorize]
        public ActionResult<decimal> Get(int id)
        {
            return balances[id];
        }

        [HttpGet("transfer")]
        [CookieAuthorize]
        public ActionResult<decimal> Get([FromQuery] int from, [FromQuery] int to, [FromQuery] decimal amount)
        {
            balances[from] -= amount;
            balances[to] += amount;
            return balances[from];
        }
        
        [HttpPost("transfer")]
        [CookieAuthorize]
        public ActionResult<decimal> Post([FromBody] TransferBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            balances[request.FromAccount] -= request.Amount;
            balances[request.ToAccount] += request.Amount;
            return balances[request.FromAccount];
        }

        [HttpPost("transfer2")]
        [CookieAuthorize]
        public ActionResult<decimal> PostForm([FromForm] TransferBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            balances[request.FromAccount] -= request.Amount;
            balances[request.ToAccount] += request.Amount;
            return balances[request.FromAccount];
        }
    }
}
