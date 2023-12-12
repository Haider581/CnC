using System.ComponentModel.DataAnnotations;

namespace CnC.Core.Audit
{
    public class UserSignIn : CnCObject
    {
        [Required]
        public int UserId { get; set; }     
    }
}
