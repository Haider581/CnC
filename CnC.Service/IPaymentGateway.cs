using CnC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CnC.Service
{
   public interface IPaymentGateway
    {
        PaymentGatewayInfo PaymentGatewayInfo { get; set; }

        PaymentGatewayResponse GenerateToken(decimal amount, string applicationNo, string paymentId, string returnUrl);

        /// <summary>
        /// CRN is unique nummber and TRN is expected a unique tracking number received from Mabna
        /// </summary>
        PaymentGatewayResponse GetResponse(HttpRequestBase request);

    }
}
