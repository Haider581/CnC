using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core.Proxy
{
    public class ProxyServer
    {
        [Key, Column(Order = 0), MaxLength(100)]
        public string Domain { get; set; }
        [Key, Column(Order = 1), MaxLength(30)]
        public string IPAddress { get; set; }
        [Required]
        public int Port { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(50)]
        public string Password { get; set; }
        [Required, MaxLength(100)]
        public string PacFile { get; set; }
        [Required]
        public bool Active { get; set; }
        public DateTime CreatedOn { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }        
    }
}
