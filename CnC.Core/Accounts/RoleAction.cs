using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Accounts
{
    public class RoleAction
    {
        [Key, Column(Order = 0)]
        [DisplayName("Role Id")]
        public int RoleId { get; set; }
        [Key, Column(Order = 1)]
        [DisplayName("Action Id")]
        public int ActionId { get; set; }
        [Required, DefaultValue(false)]
        [DisplayName("Is Allowed")]
        public bool IsAllowed { get; set; }
        [Required]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
        [Required, DefaultValue(false)]
        [DisplayName("Is Default")]
        public bool IsDefault { get; set; }
    }
}
