using CnC.Core.Accounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CnC.Core.Proxy
{
    public class ProxyServerUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        [Required, MaxLength(100)]
        public string Domain { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastAccessedOn { get; set; }

        #region Custom Properties

        [NotMapped]
        public ProxyServer ProxyServer { get; set; }
        [NotMapped]
        public User User { get; set; }

        #endregion
    }
}