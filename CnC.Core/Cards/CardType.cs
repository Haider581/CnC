using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Cards
{
    public class CardType : CnCObject
    {
        /// <summary>
        /// Card Name e.g. GOLD,BLACK or PLATINUM
        /// </summary>
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50)]

        public string Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Fee { get; set; }
        [Required, DefaultValue(false)]
        public bool IsReloadable { get; set; }
        /// <summary>
        /// Total Amount Customer can reload in the card
        /// </summary>
        [Required]
        [DataType(DataType.Currency)]
        public decimal ReloadLimit { get; set; }
        /// <summary>
        /// Total Amount Customer can reload in the Card within days
        /// </summary>
        [Required]        
        public int ReloadDayLimit { get; set; }
        /// <summary>
        /// Maximum Reload Allowed
        /// </summary>
        [Required]
        public int MaximumReload { get; set; }
        /// <summary>
        /// Minimum Top Up Amount at a time
        /// </summary>
        [Required]
        [DataType(DataType.Currency)]
        public decimal MinimumReloadAtATime { get; set; }
        /// <summary>
        /// Maximum Top Up Amount at a time
        /// </summary>
        [Required]
        [DataType(DataType.Currency)]
        public decimal MaximumReloadAtATime { get; set; }
        /// <summary>
        /// Card maximum quantity can be issued to Customer
        /// </summary>
        [Required]
        public int MaxQuantity { get; set; }
        [Required]
        public bool IsProofOfSourceOfFundsRequired { get; set; }
        [Required]
        public bool IsProofOfAddressRequired { get; set; }
        /// <summary>
        /// Requirements to apply for the Card e.g. Source of Funds proof required for Black Card
        /// </summary>
        [MaxLength(250)]
        public string Requirements { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Top Up Service Fee Percentage
        /// </summary>
        [Required]
        public decimal TopUpServiceFeePercentage { get; set; }

        /// <summary>
        /// Minimum Top Up Service Fee Amount
        /// </summary>
        [Required]
        public decimal TopUpServiceFeeMinimum { get; set; }
    }
}
