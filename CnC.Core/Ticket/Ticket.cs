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
    public class Ticket : CnCObject
    {
       
        
        [Required]
        [MaxLength (100)]
        public string Subject { get; set; }
        [Required]
        [MaxLength (80)]
        public string TicketNo { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public DateTime? LastSeenByUserDate { get; set; }
        public int? LastSeenByUserId { get; set; }
        [NotMapped]
        public List<TicketMessage> Messages { get; set; }
        [NotMapped]
        public List<AssignTicket> TicketAssignedToStaff { get; set; }
        [Required, DefaultValue(false)]
        public bool IsResolved { get; set; }
       
        [MaxLength (500)]
        public string FilePath { get; set; }
        [NotMapped]
        public QuickTicket QuickTicket { get; set; }
        [NotMapped]
        public List<TicketProcess> TicketProcess { get; set; }
        [Required]
        public int SeverityId { get; set; }
        [Required, DefaultValue(false)]
        public bool Isassign { get; set; }
       [NotMapped]
        public TicketMessage Message { get; set; }

    }
}
