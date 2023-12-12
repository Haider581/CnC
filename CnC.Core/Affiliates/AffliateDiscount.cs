using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core.Affiliates
{
    public class AffiliateDiscount : CnCObject
    {
        [Required]
        public int AffiliateId { get; set; }
        [Required]
        public int CardTypeId { get; set; }
        [Required]
        public decimal Discount { get; set; }
        [Required]
        public bool IsPercent { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartOn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndOn { get; set; }
        [Required]
        public bool Active { get; set; }

        #region CustomerProperties
        public string[] CardTypeIds { get; set; }
        #endregion

    }
}
