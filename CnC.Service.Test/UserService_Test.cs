using CnC.Core;
using CnC.Core.Accounts;
using CnC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Service.Test
{
    public class UserService_Test
    {
        private EntityContext context;
        public void ValidateUser_Test(User user, bool isCustomer)
        {
            var commonService = new CommonService();

            if (string.IsNullOrEmpty(user.Email) || !commonService.IsValidEmail(user.Email))
                throw new ArgumentException("Email is either missing or invalid");

            if (user.Email.Length > 50)
                throw new ArgumentException("Email length cannot be greater than 50");

            if (string.IsNullOrEmpty(user.FirstName))
                throw new ArgumentException("First Name is required");

            if ((user.FirstName.Length < 2 || user.FirstName.Length > 50))
                throw new ArgumentException(string.Format("First Name length must be between {0} and {1}"
                    , 2, 50));

            if (isCustomer && string.IsNullOrEmpty(user.FirstNameInCustomerLanguage))
                throw new ArgumentNullException("First Name in Customer Language is required");

            if (isCustomer && (user.FirstNameInCustomerLanguage.Length < 2 || user.FirstNameInCustomerLanguage.Length > 50))
                throw new ArgumentNullException(string.Format("First Name in Customer Language length must be between {0} and {1}"
                    , 2, 50));

            if (string.IsNullOrEmpty(user.LastName))
                throw new ArgumentNullException("Last Name is required");

            if (user.LastName.Length < 2 || user.LastName.Length > 50)
                throw new ArgumentNullException(string.Format("Last Name length must be between {0} and {1}"
                    , 2, 50));

            if (isCustomer && string.IsNullOrEmpty(user.LastNameInCustomerLanguage))
                throw new ArgumentNullException("Last Name in Customer Language is required");

            if (isCustomer && (user.LastNameInCustomerLanguage.Length < 2 || user.LastNameInCustomerLanguage.Length > 50))
                throw new ArgumentNullException(string.Format("Last Name in Customer Language length must be between {0} and {1}"
                    , 2, 50));

            if (user.RoleId <= 0)
                throw new ArgumentNullException("User Role is required");

            UserStatus userStatus;
            if (Enum.TryParse<UserStatus>(user.Status.ToString(), out userStatus) == false)
                throw new ArgumentNullException("Status is required");

            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentNullException("Username is required");
        }
        public Role GetRole_Test(string name)
        {
            try
            {
                var context = new EntityContext();
                return context.Roles.SingleOrDefault(r => r.Name.ToLower() == name.ToLower());
            }
            catch (Exception excep) { }
            return null;
        }

    }
}
