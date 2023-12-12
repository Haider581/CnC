using CnC.Core.Cards;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.TopUps
{
    public class TopUpRequest : CnCObject
    {
        [Required]
        public int TopUpRequestFormId { get; set; }
        /// <summary>
        /// Card Request Id is Card Id
        /// </summary>
        [Required]
        public int CardId { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal ServiceFee { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? UploadedOn { get; set; }
        public DateTime? ApprovedOn { get; set; }
        [MaxLength(250)]
        public string FailureReason { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public decimal? ServiceExchangeRateFee { get; set; }
        #region Custom Properties

        [NotMapped]
        public Card Card { get; set; }

        [NotMapped]
        public TopUpRequestForm TopUpRequestForm { get; set; }

        [NotMapped]
        public CardRequestStatus Status { get; set; }

        [NotMapped]
        public string StatusString { get; set; }

        #endregion

    }
}
