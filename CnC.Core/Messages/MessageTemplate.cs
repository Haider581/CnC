using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core.Messages
{
    public class MessageTemplate
    {
        #region Properties

        /// <summary>
        /// Gets or sets the message template identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }

        #endregion
    }
}
