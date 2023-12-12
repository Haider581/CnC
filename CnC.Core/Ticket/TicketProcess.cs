using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CnC.Core
{
   public class TicketProcess: CnCObject
    {
        [Required]
        public int TicketId { get; set; }
        [Required]
        public int UserId { get; set; }

        public string Comments { get; set; }
        [Required]
        public int TicketStatusId { get; set; }
        
       

        
    }
}
