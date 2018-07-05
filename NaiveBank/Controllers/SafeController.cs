using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace NaiveBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class SafeController : ControllerBase
    {
        private readonly static ConcurrentDictionary<int, decimal> balances = new ConcurrentDictionary<int, decimal>();

        private readonly IAntiforgery antiforgery;
        private readonly IAntiforgeryTokenSerializer tokenSerializer;
        private readonly AntiforgeryOptions options;

        static SafeController()
        {
            balances.TryAdd(1, 100m);
            balances.TryAdd(2, 0m);
        }

        public SafeController(IAntiforgery antiforgery, IAntiforgeryTokenSerializer tokenSerializer, IOptions<AntiforgeryOptions> optionsAccessor)
        {
            this.antiforgery = antiforgery;
            this.tokenSerializer = tokenSerializer;
            this.options = optionsAccessor.Value;
        }

        [HttpGet("balances")]
        public ActionResult<ConcurrentDictionary<int, decimal>> Get()
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

            balances[request.FromAccount] -= request.Amount;
            balances[request.ToAccount] += request.Amount;
            return balances[request.FromAccount];
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
        public ActionResult<dynamic> ValidateAntiforgeryToken()
        {
            try
            {
                var cookieToken = this.HttpContext.Request.Cookies[this.options.Cookie.Name];
                var requestToken = this.HttpContext.Request.Headers[this.options.HeaderName];

                byte[] cookieSecurityToken = this.tokenSerializer.Deserialize(cookieToken).SecurityToken.GetData();
                byte[] requestSecurityToken = this.tokenSerializer.Deserialize(requestToken).SecurityToken.GetData();

                string cookieSecurityTokenBase64String = Convert.ToBase64String(cookieSecurityToken);
                string requestSecurityTokenBase64String = Convert.ToBase64String(requestSecurityToken);

                return new
                {
                    CookieSecurityToken = cookieSecurityTokenBase64String,
                    RequestSecurityToken = requestSecurityTokenBase64String,
                    Equal = cookieSecurityTokenBase64String == requestSecurityTokenBase64String
                };
            }
            catch (Exception e)
            {
                return new { CookieName = this.options.Cookie.Name, this.options.HeaderName, Exception = e.ToString() };
            }
        }
    }
}
