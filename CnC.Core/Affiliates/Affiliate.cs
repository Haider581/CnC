using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CnC.Core.Affiliates
{
    public class Affiliate : CnCObject
    {
        [Required]   
        public int AddressId { get; set; }
        [MaxLength(500), DataType(DataType.MultilineText)]
        public string Comment { get; set; }
        [MaxLength(50)]
        public string FriendlyUrlName { get; set; }
        [Required]
        public bool Deleted { get; set; }
        [Required]
        public bool Active { get; set; }
        public decimal Commision { get; set; }

        [NotMapped]
        public Address Address { get; set; }

        [NotMapped]
        public Commission Commission { get; set; }
    }
}
