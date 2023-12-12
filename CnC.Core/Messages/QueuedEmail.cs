using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CnC.Core.Messages
{
    public class QueuedEmail : CnCObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        [Required, DefaultValue(1)]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the From property
        /// </summary>        
        [EmailAddress, Required, MaxLength(100)]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the To property
        /// </summary>        
        [EmailAddress, Required, MaxLength(100)]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the ToName property
        /// </summary>
        [Required, MaxLength(50)]
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the CC
        /// </summary>
        [MaxLength(100)]
        public string CC { get; set; }

        /// <summary>
        /// Gets or sets the Bcc
        /// </summary>
        [MaxLength(100)]
        public string Bcc { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [Required, MaxLength(100)]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        [Required]
        [AllowHtml]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        [Required, DefaultValue(0)]
        public int SendTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOn { get; set; }

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        [Required]
        public int EmailAccountId { get; set; }

        #endregion
    }
}
