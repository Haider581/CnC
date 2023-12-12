using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core.Discounts
{
    public class DiscountPromotion : CnCObject
    {
        [Required]
        public decimal Discount { get; set; }

        [Required]
        public int CardTypeId { get; set; }

        [Required]
        public bool IsPercent { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartOn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndOn { get; set; }
    }
}
