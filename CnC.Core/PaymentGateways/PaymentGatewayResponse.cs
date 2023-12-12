using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core
{
    [NotMapped]
    public class PaymentGatewayResponse
    {
        public bool IsSuccess { get; set; }

        /// <summary>
        /// In case of success html script will return
        /// otherwise failure reason
        /// </summary>
        public string Message { get; set; }

        public string TransactionNumber { get; set; }
    }
}
