using CnC.Core;
using CnC.Service.Token;
using CnC.Service.Verify;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CnC.Service.PaymentGateway.Kish
{
    public class KishPaymentGateway : IPaymentGateway
    {
        #region Help

        // Merchant Id: 4 Capital Letters e.g AB12
        // Amount: Minimum 1000
        // Payment Id: Maximum 12 Letters
        // Invoice/Application Number: Maximum 12 Letters
        // Special Payment Id: Maximum 25 Letters
        // Result Code: the code is the result of transaction implementation condition. 
        // Code 100, for instant, means the transaction is done correctly. 
        // Except code 100, shows error code.Recourse to table 9 in attachment section.
        // Reference Id: this code is transaction address which, is unique. Reference Id is for tracking in bank network. 

        #endregion

        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        public PaymentGatewayInfo PaymentGatewayInfo { get; set; }
        private string generatedToken;

        #region Generate Token
       
        public PaymentGatewayResponse GenerateToken(decimal amount, string applicationNo, string paymentId
            , string returnUrl)
        {
            PaymentGatewayResponse response = new PaymentGatewayResponse();

            try
            {
                TokensClient client = new TokensClient();
                tokenResponse tokenResp = client.MakeToken(amount.ToString(), PaymentGatewayInfo.MerchantId
                    , applicationNo, paymentId, "", returnUrl, "");

                if (tokenResp.result)
                {
                    response.IsSuccess = true;
                    generatedToken = tokenResp.token;
                    response.Message = CreateHtmlFormToPost(tokenResp.token, paymentId);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = tokenResp.message;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                response.IsSuccess = false;
                response.Message = "Failed to connect with payment gateway";
            }

            return response;
        }

        private string CreateHtmlFormToPost(string receivedToken, string paymentId)
        {
            return "<form action='https://ikc.shaparak.ir/TPayment/Payment/Index' id='form1' method='post'>"
                + "<input type='hidden' name='token' value='" + receivedToken
                + "' /><input type='hidden' name='merchantId' value='" + PaymentGatewayInfo.MerchantId
                + "' /><input type='hidden' name='PaymentId' value='" + paymentId
                + "' /></form><script>document.getElementById('form1').submit();</script>";
        }

        #endregion Generate Token

        #region Get Response

        public PaymentGatewayResponse GetResponse(HttpRequestBase request)
        {
            PaymentGatewayResponse response = new PaymentGatewayResponse();

            try
            {
                string token = request.Form["token"];
                string merchantId = request.Form["merchantId"];
                string resultCode = request.Form["resultCode"];
                string referenceId = request.Form["referenceId"];

                using (var VerifyService = new VerifyClient())
                {
                    if (!string.IsNullOrEmpty(resultCode) && resultCode == "100")
                    {
                        var res = VerifyService.KicccPaymentsVerification(token, merchantId, referenceId
                            , PaymentGatewayInfo.EncryptionKey1);

                        if (res > 0)
                        {
                            //  Verification Passed , your statements Goes here
                            response.IsSuccess = true;
                            response.TransactionNumber = referenceId;
                            response.Message = "Payment has been processed successfully";
                        }
                        else
                        {
                            //  Verification Failed , your statements Goes here
                            response.IsSuccess = false;
                            response.Message = "Failed to process payment";
                        }
                    }
                    else if (!string.IsNullOrEmpty(resultCode) && resultCode == "110")
                    {
                        response.IsSuccess = false;
                        response.Message = "Card owner refused the payment";
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Failed to process payment";
                    }

                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                response.IsSuccess = false;
                response.Message = "Unable to process payment";
            }
            return response;
        }

        #endregion Get Response
    }
}
