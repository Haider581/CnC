using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core.Messages
{
    public class EmailAccount
    {
        #region Properties

        /// <summary>
        /// Gets or sets the email account identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets an email address
        /// </summary>
        [EmailAddress, Required]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets an email display name
        /// </summary>

        [Required, MaxLength(50)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets an email host
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets an email port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets an email user name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets an email password
        /// </summary>
        [Required, MaxLength(250)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection
        /// </summary>
        [DefaultValue(false)]
        public bool EnableSSL { get; set; }        

        #endregion
    }
}
