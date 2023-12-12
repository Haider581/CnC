
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CnC.Core;

namespace CnC.Core
{
    public class QuickTicket : CnCObject
    {
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public int PriorityId { get; set; }
    }
}
