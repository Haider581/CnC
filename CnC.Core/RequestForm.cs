using CnC.Core.Customers;
using CnC.Core.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core
{
    public class RequestForm : CnCObject
    {
        [Required]
        public int TypeId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public decimal ExchangeRate { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public bool? IsCancelled { get; set; }
        public DateTime? SentToCardIssuerOn { get; set; }

        #region Custom Properties

        [NotMapped]
        public RequestFormType Type { get; set; }

        [NotMapped]
        public List<Payment> Payments { get; set; }

        [NotMapped]
        public Customer Customer { get; set; }

        [NotMapped]
        public RequestFormStatus Status { get; set; }

        [NotMapped]
        public string ApplicationNumber
        {
            get
            {
                return CreatedOn.ToString("yyyyMMddHHmmss") + Id;
            }
        }

        [NotMapped]
        public decimal PaymentToPay { get; set; }

        [NotMapped]
        public decimal PaymentPaid { get; set; }

        #endregion
    }
}
