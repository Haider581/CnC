using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Payments
{
    public class Payment : CnCObject
    {
        [Required]
        public int RequestFormId { get; set; }
        [Required]
        public int PaymentMethodId { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        #region Payment Method Bank

        [MaxLength(50)]
        public string TransactionNo { get; set; }
        [MaxLength(50)]
        public string TransactionAccountNo { get; set; }
        [MaxLength(50)]
        public string TransactionName { get; set; }     
        public DateTime? ConfirmedOn { get; set; }
        public DateTime? ReConfirmedOn { get; set; }
        [MaxLength(200)]
        public string ConfirmationFailureReason { get; set; }

        #endregion

        #region Customer Properties

        [NotMapped]
        public PaymentMethod PaymentMethod { get; set; }

        [NotMapped]
        public RequestForm RequestForm { get; set; }

        [NotMapped]
        public string StatusString { get; set; }

        #endregion
    }
}
