using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Accounts
{
    public class Role : CnCObject
    {
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50)]
        [DisplayName("Name")]
        public string Name { get; set; }
        [MaxLength(500)]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DefaultValue(0)]
        [DisplayName("Can View Online Records")]
        public bool CanViewOnlineRecords { get; set; }

        [DefaultValue(0)]
        public bool CanViewBankRecords { get; set; }


        #region CustomerProperties
        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayName("Created Date From")]
        public DateTime? CreatedDateFrom { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayName("Created Date To")]
        public DateTime? CreatedDateTo { get; set; }
        #endregion
    }
}
