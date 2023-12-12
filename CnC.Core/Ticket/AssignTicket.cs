using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace CnC.Core
{
    public class AssignTicket : CnCObject
    {
        
       
        [Required]
        public int UserId { get; set; }
        [Required]
        public int TicketID { get; set; }
       
        [NotMapped]
        public bool IsAssign { get; set; }

       


    }
}
