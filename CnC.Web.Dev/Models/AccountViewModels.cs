using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CnC.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        //[Display(Name = "Email")]
        [Display(Name = "Username")]
        //[EmailAddress]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
