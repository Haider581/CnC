using CnC.Core.Accounts;
using CnC.Core.Cards;
using CnC.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Customers
{
    public class Customer
    {
        [NotMapped]
        public int Id { get { return UserId; } }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        [Required]
        public int GenderId { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [MaxLength(50)]
        public string DateOfBirthInCustomerLanguage { get; set; }
        [Required]
        public int MaritalStatusId { get; set; }
        [Required, MaxLength(20), Index(IsUnique = true)]
        public string NIC { get; set; }
        [Required, MaxLength(20), Index(IsUnique = true)]
        public string PassportNo { get; set; }
        [Required, MaxLength(100)]
        public string Nationality { get; set; }
        [Required, MaxLength(500)]
        public string Address { get; set; }
        [Required, MaxLength(500)]
        public string AddressInCustomerLanguage { get; set; }
        [MaxLength(500)]
        public string Address2 { get; set; }
        [MaxLength(500)]
        public string Address2InCustomerLanguage { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required, MaxLength(10)]
        public string PostalCode { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required, MaxLength(20)]
        public string ContactNo { get; set; }
        [MaxLength(100)]
        public string NICDoc { get; set; }
        [Required, MaxLength(100)]
        public string NICDocInCustomerLanguage { get; set; }
        [MaxLength(100)]
        public string PassportDoc { get; set; }
        [Required, MaxLength(100)]
        public string PassportDocInCustomerLanguage { get; set; }
        [MaxLength(100)]
        public string ProofOfAddressDocType { get; set; }
        //[MaxLength(200)]
        //public string ProofOfAddressDoc { get; set; }
        [MaxLength(200)]
        public string ProofOfAddressDocInCustomerLanguage { get; set; }
        [MaxLength(100)]
        public string ProofOfSourceOfFundsDocType { get; set; }
        //[MaxLength(200)]
        //public string ProofOfSourceOfFundsDoc { get; set; }
        [MaxLength(200)]
        public string ProofOfSourceOfFundsDocInCustomerLanguage { get; set; }
        [MaxLength(100)]
        public string CustomerSignedForm { get; set; }
        [MaxLength(100)]
        public string CustomerSignedFormInCustomerLanguage { get; set; }
        //[MaxLength(100)]
        //public string AuthoritySignedForm { get; set; }
        //[MaxLength(100)]
        //public string AuthoritySignedFormInCustomerLanguage { get; set; }
        [Required]
        public int CurrencyId { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [MaxLength(100)]
        public string SecurityQuestion { get; set; }
        [MaxLength(100)]
        public string Answer { get; set; }
        public DateTime? ValidatedOn { get; set; }
        [MaxLength(200)]
        public string ValidationFailureReason { get; set; }
        //public DateTime? SentToCardIssuerOn { get; set; }
        public DateTime? CardIssuerRespondedOn { get; set; }
        [MaxLength(500)]
        public string DeclinedReason { get; set; }
        [MaxLength(200)]
        public string BillingAddress { get; set; }
        [EmailAddress, MaxLength(50)]
        public string EmailForShopping { get; set; }
        [MaxLength(20)]
        public string VerificationCode { get; set; }
        public int? AffiliateId { get; set; }
        public DateTime? VerificationCodeExpireOn { get; set; }
        public int? CustomerRegistrationMode { get; set; }

        #region Custom Properties

        [NotMapped]
        public User User { get; set; }

        [NotMapped]
        public Gender Gender
        {
            get
            {
                return (Gender)GenderId;
            }
        }

        [NotMapped]
        public MaritalStatus MaritalStatus
        {
            get
            {
                return (MaritalStatus)MaritalStatusId;
            }
        }

        [NotMapped]
        public Country Country { get; set; }

        [NotMapped]
        public City City { get; set; }

        [NotMapped]
        public Language Language { get; set; }

        [NotMapped]
        public Currency Currency { get; set; }

        [NotMapped]
        public CustomerStatus Status { get; set; }

        [NotMapped]
        public string StatusString { get; set; }

        [NotMapped]
        public List<CardRequestForm> CardRequestForms { get; set; }

        #endregion

    }
}
