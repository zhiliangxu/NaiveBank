using System.ComponentModel.DataAnnotations;

namespace NaiveBank
{
    public class TransferBalanceRequest
    {
        [Required]
        public string ToAccount { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
