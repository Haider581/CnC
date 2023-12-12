using CnC.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core.Messages
{
    public class LocalizedMessageTemplate
    {
        #region Properties
        /// <summary>
        /// Gets or sets the localized message template identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the message template identifier
        /// </summary>
        [Required]
        public int MessageTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        [Required]
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the BCC Email addresses
        /// </summary>
        [EmailAddress, MaxLength(100)]
        public string BccEmailAddresses { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [Required, MaxLength(100)]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the template is active
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        [Required]
        public int EmailAccountId { get; set; }

        #endregion

        #region Custom Properties

        public EmailAccount EmailAccount { get; set; }
        public MessageTemplate MessageTemplate { get; set; }
        public Language Language { get; set; }

        #endregion Custom Properties
    }
}
