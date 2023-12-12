using CnC.Core.Customers;
using CnC.Core.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Cards
{
    public class CardRequest : CnCObject
    {
        [Required]
        public int CardRequestFormId { get; set; }
        [Required]
        public int CardTypeId { get; set; }
        /// <summary>
        /// There will be one card request for each card that is why it will always be 1, so at the moment it is not required
        /// </summary>
        [Required]
        public int CardQty { get; set; }
        public decimal? ServiceExchangeFee { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Fee { get; set; }
        public DateTime? CardIssuerRespondedOn { get; set; }
        [MaxLength(500)]
        public string DeclinedReason { get; set; }
        public DateTime? DispatchedToTBOOn { get; set; }
        public DateTime? DispatchedToCustomerOn { get; set; }
        public DateTime? DeliveredToCustomerOn { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }

        public decimal? Discount { get; set; }
        #region Custom Properties

        [NotMapped]
        public Card Card { get; set; }

        [NotMapped]
        public CardType CardType { get; set; }

        [NotMapped]
        public RequestForm CardRequestForm { get; set; }

        [NotMapped]
        public CardRequestStatus Status { get; set; }

        [NotMapped]
        public string StatusString { get; set; }

        #endregion
    }
}
