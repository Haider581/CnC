using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Cards
{
    [NotMapped]
    public class CardPaymentTransaction
    {        
        /// <summary>
        /// Transaction Amount in Paid Currency
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Transaction Amount in Account Currency
        /// </summary>
        public decimal AccountCurrencyAmount { get; set; }
        public decimal AccountServiceFee { get; set; }
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Transaction Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Transaction Type Description
        /// If Description is null then use it
        /// </summary>
        public string TransactionTypeDescription { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDebit { get; set; }
        public string Status { get; set; }
    }
}
