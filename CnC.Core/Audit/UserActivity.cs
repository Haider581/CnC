using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Audit
{
    public class UserActivity : CnCObject
    {
        [Required]
        public int UserId { get; set; }
        public int? PerformedForUserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Action { get; set; }

        [Required]
        public string IpAddress { get; set; }

        [Required]
        public string BrowserName { get; set; }

        [Required]
        public string MachineName { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDateFrom { get; set; }
        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDateTo { get; set; }

    }
}
