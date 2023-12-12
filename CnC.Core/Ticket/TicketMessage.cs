using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using CnC.Core.Accounts;

namespace CnC.Core
{
    public class TicketMessage : CnCObject
    {
        
        [Required]
        public int TicketId { get; set; }
        [Required]
        public int UserId { get; set; }
       
        [Required]
        [MaxLength (2000)]
        public string Message { get; set; }
        [NotMapped]
        public User User { get; set; }

        
    }
}
