using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Smart.BL.Accounts
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        public string HashedPassword { get; set; }
        public string HashKey { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }        
    }
}
