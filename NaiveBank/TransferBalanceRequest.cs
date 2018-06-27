
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NaiveBank
{
    public class TransferBalanceRequest
    {
        [JsonProperty("from_account")]
        [Required]
        public int FromAccount { get; set; }

        [JsonProperty("to_account")]
        [Required]
        public int ToAccount { get; set; }

        [JsonProperty("amount")]
        [Required]
        public decimal Amount { get; set; }
    }
}
