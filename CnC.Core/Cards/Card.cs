using CnC.Core.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Cards
{
    public class Card
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CardRequestId { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(20)]        
        public string Number { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        public string CardServiceProviderCardId { get; set; }

        public string CardServiceProviderClientId { get; set; }
        [Required]
        [MaxLength(4)]
        public string ExpiryDate { get; set; }
        [Required]
        public int Status { get; set; }

        [NotMapped]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        #region Custom Properties

        [NotMapped]
        public CardRequest CardRequest { get; set; }
        [NotMapped]
        public Customer Customer { get; set; }
        [NotMapped]
        public List<CardPaymentTransaction> PaymentTransactions { get; set; }

        #endregion
    }
}
