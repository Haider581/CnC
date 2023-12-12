using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core
{
    [Table("PaymentGatewaysInfo")]
    public class PaymentGatewayInfo : CnCObject
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(1000)]
        public string EncryptionKey1 { get; set; }
        [MaxLength(1000)]
        public string EncryptionKey2 { get; set; }
        [Required, MaxLength(100)]
        public string RetrunUrl { get; set; }
        [Required, MaxLength(100)]
        public string RedirectUrl { get; set; }
        [Required, MaxLength(50)]
        public string MerchantId { get; set; }
        [MaxLength(50)]
        public string TerminalId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required, MaxLength(150)]
        public string ClassPath { get; set; }
        public int? GatewayType { get; set; }
    }
}
