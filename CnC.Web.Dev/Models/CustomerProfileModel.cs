using CnC.Core;
using CnC.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CnC.Web.Models
{
    public class CustomerProfileModel
    {
      
        public int Id { get { return UserId; } }
        public int UserId { get; set; }
               
        public DateTime DateOfBirth { get; set; }
      
        public string DateOfBirthInCustomerLanguage { get; set; }
      
        public string NIC { get; set; }
        
        public string PassportNo { get; set; }
       
        public string Nationality { get; set; }
        
        public string Address { get; set; }
      
        public string AddressInCustomerLanguage { get; set; }
      
        public string Address2 { get; set; }
       
        public string Address2InCustomerLanguage { get; set; }
        
              
        public string PostalCode { get; set; }
               
        public string ContactNo { get; set; }
        
        public string BillingAddress { get; set; }
       
        public string EmailForShopping { get; set; }
       
       
        #region Custom Properties

        public string FirstName { get; set; }
              
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public string Gender { get; set; }

        public string MaritalStatus { get; set; }
        
        public string CountryName { get; set; }
        
        public string CityName { get; set; }

       
        
        
        #endregion
    }
}