using System;
using System.Collections.Generic;
using CnC.Core;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Accounts
{
    public class User : CnCObject
    {
        [Required, Index(IsUnique = true), MaxLength(100)]
        public string Username { get; set; }
        [Required, MinLength(3), MaxLength(50), DisplayName("First Name")]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string FirstNameInCustomerLanguage { get; set; }
        [Required]
        [MinLength(3), MaxLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [MaxLength(50)]
        public string LastNameInCustomerLanguage { get; set; }
        [EmailAddress, Required]
        [Index(IsUnique = true)]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        [MaxLength(250)]
        [DataType(DataType.Password)]
        public string HashedPassword { get; set; }
        [Required]
        [MaxLength(100)]
        public string HashKey { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public int RoleId { get; set; }

        [DefaultValue(0)]

        public bool ChangePasswordRequired { get; set; }

        #region Custom Properties

        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayName("Created Date From")]
        public DateTime? CreatedDateFrom { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayName("Created Date To")]
        public DateTime? CreatedDateTo { get; set; }
        [NotMapped]
        public string FullName { get { return FirstName + " " + LastName; } }

        [NotMapped]
        public string FullNameInCustomerLanguage { get { return FirstNameInCustomerLanguage + " " + LastNameInCustomerLanguage; } }

        [NotMapped]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [NotMapped]
        [Compare("NewPassword")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        public Role Role { get; set; }

        [NotMapped]
        public bool IsCustomer
        {
            get
            {
                if (Role == null)
                    return false;
                return Role.Name.Equals("Customer", StringComparison.OrdinalIgnoreCase);
            }
        }

        [NotMapped]
        public bool IsRahyab
        {
            get
            {
                if (Role == null)
                    return false;
                return Role.Name.Equals("RAHYAB", StringComparison.OrdinalIgnoreCase);
            }
        }


        [NotMapped]
        public bool IsTBOSuperAgent
        {
            get
            {
                if (Role == null)
                    return false;
                return Role.Name.Equals("TBOSuperAgent", StringComparison.OrdinalIgnoreCase);
            }
        }

        [NotMapped]
        public bool IsTBOAdmin
        {
            get
            {
                if (Role == null)
                    return false;
                return Role.Name.Equals("TBOAdmin", StringComparison.OrdinalIgnoreCase);
            }
        }

        [NotMapped]
        public bool IsTBOAgent
        {
            get
            {
                if (Role == null)
                    return false;
                return Role.Name.Equals("TBOAgent", StringComparison.OrdinalIgnoreCase);
            }
        }

        [NotMapped]
        public UserStatus UserStatus
        {
            get
            {
                return (UserStatus)Status;
            }
        }

        #endregion
    }
}
