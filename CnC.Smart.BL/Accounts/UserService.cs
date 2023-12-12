using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
using CC.Smart.BL.Accounts;

namespace CC.Smart.BL.Accounts
{
    public class UserService
    {
        public User Authenticate(string userName, string passWord)
        {
            return new User() { Id = 1, FirstName = "Sabir", LastName = "Hussain", Email = "abc.yahoo.com", Status = 1 };
        }

        public User GetUser(int userId)
        {
            return new User() { Id = 1, Email = "abc@yahoo.com", Status = 1, FirstName = "Ali", LastName = "Khan" };
        }
        
        public List<User> GetUsers()
        {
            var users = new List<User>();
            users.Add(new User() { Id = 1, CreatedOn = DateTime.UtcNow, Email = "Sabir@yahoo.com", FirstName = "Sabir", HashedPassword = "1233", LastName = "Hussain", Status = 1, Username = "SabirHussain" });
            users.Add(new User() { Id = 2, CreatedOn = DateTime.UtcNow, Email = "Azeem@yahoo.com", FirstName = "Azeem", HashedPassword = "1233", LastName = "Ravi", Status = 1, Username = "AzeemRavi" });
            users.Add(new User() { Id = 3, CreatedOn = DateTime.UtcNow, Email = "Daud@yahoo.com", FirstName = "Muhammad", HashedPassword = "1233", LastName = "Daud", Status = 1, Username = "MUhammadDaud" });
            users.Add(new User() { Id = 4, CreatedOn = DateTime.UtcNow, Email = "Waleed@yahoo.com", FirstName = "Waleed", HashedPassword = "1233", LastName = "Iqbal", Status = 0, Username = "WaleedIqbal" });
            
            return users;
        }
        
        public int AddUser(User user)
        {
            return 1;
        }
    }

}