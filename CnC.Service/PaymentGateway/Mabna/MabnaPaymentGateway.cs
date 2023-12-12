using CnC.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CnC.Service.PaymentGateway.Mabna
{
    public class MabnaPaymentGateway : IPaymentGateway
    {
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
                string crn = applicationNo;

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                rsa.FromXmlString(PaymentGatewayInfo.EncryptionKey2);

                var merchantId = PaymentGatewayInfo.MerchantId;
                var referalAddress = returnUrl;
                var terminalId = PaymentGatewayInfo.TerminalId;

                byte[] signMain = rsa.SignData(System.Text.Encoding.UTF8.GetBytes(
                    Convert.ToInt32(amount).ToString() + crn + merchantId + referalAddress + terminalId),
                    new SHA1CryptoServiceProvider());

                string signature = Convert.ToBase64String(signMain);

                RSACryptoServiceProvider cipher = new RSACryptoServiceProvider();
                cipher.FromXmlString(PaymentGatewayInfo.EncryptionKey1);

                byte[] data = Encoding.UTF8.GetBytes(amount.ToString());
                byte[] cipherText = cipher.Encrypt(data, false);
                string encryptedAmount = Convert.ToBase64String(cipherText);

                data = Encoding.UTF8.GetBytes(merchantId);
                cipherText = cipher.Encrypt(data, false);
                string encryptedMId = Convert.ToBase64String(cipherText);

                data = Encoding.UTF8.GetBytes(crn);
                cipherText = cipher.Encrypt(data, false);
                string encryptedCRN = Convert.ToBase64String(cipherText);

                data = Encoding.UTF8.GetBytes(referalAddress);
                cipherText = cipher.Encrypt(data, false);
                string encryptedREFERALADRESS = Convert.ToBase64String(cipherText);

                data = Encoding.UTF8.GetBytes(terminalId);
                cipherText = cipher.Encrypt(data, false);
                string encryptedTID = Convert.ToBase64String(cipherText);

                TokenService.Token _services = new TokenService.Token();
                TokenService.tokenDTO _TokenParm = new TokenService.tokenDTO();
                _TokenParm.AMOUNT = encryptedAmount;
                _TokenParm.CRN = encryptedCRN;
                _TokenParm.MID = encryptedMId;
                _TokenParm.REFERALADRESS = encryptedREFERALADRESS;
                _TokenParm.SIGNATURE = signature;
                _TokenParm.TID = encryptedTID;

                TokenService.tokenResponse _TokenResponse = _services.reservation(_TokenParm);

                int result = _TokenResponse.result;
                
                generatedToken = null;

                if (result == 0)
                {
                    response.IsSuccess = true;
                    generatedToken = _TokenResponse.token;
                    response.Message = CreateHtmlFormToPost(generatedToken);
                }
                else
                {
                    response.IsSuccess = false;
                    // In case of failure Token has the reason
                    response.Message = _TokenResponse.token;
                }                
            }
            catch (Exception exception)
            {
                log.Error(exception);
                response.IsSuccess = false;
                response.Message = "Unable to connect with payment gateway";
            }

            return response;
        }

        private string CreateHtmlFormToPost(string receivedToken)
        {
            var url = PaymentGatewayInfo.RedirectUrl; //"http://localhost:13049/Home/Test"; //
            StringBuilder htmlForm = new StringBuilder();
            htmlForm.AppendLine("<html>");
            htmlForm.AppendLine(String.Format("<body onload='document.forms[\"{0}\"].submit()'>", "form1"));
            htmlForm.AppendLine(String.Format("<form id='{0}' method='POST' target='_top' action='{1}'>"
                , "form1", url));
            htmlForm.AppendLine("<input type='hidden' name='TOKEN' value='" + receivedToken + "' />");
            htmlForm.AppendLine("</form>");
            htmlForm.AppendLine("</body>");
            htmlForm.AppendLine("</html><script>document.getElementById('form1').submit();</script>");
            return htmlForm.ToString();
        }

        #endregion Generate Token

        #region Get Response

        public PaymentGatewayResponse GetResponse(HttpRequestBase request)
        {
            PaymentGatewayResponse response = new PaymentGatewayResponse();

            try
            {
                string responseCode = request.Form["RESCODE"].ToString();
                string amount = request.Form["AMOUNT"].ToString();
                string crn = request.Form["CRN"].ToString();
                string trn = request.Form["TRN"].ToString();

                ServicePointManager.ServerCertificateValidationCallback =
                            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                if (responseCode == "00")
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    CspParameters _cpsParameter;
                    RSACryptoServiceProvider RSAProvider;

                    _cpsParameter = new CspParameters();
                    _cpsParameter.Flags = CspProviderFlags.UseMachineKeyStore;

                    RSAProvider = new RSACryptoServiceProvider(1024, _cpsParameter);
                    RSACryptoServiceProvider.UseMachineKeyStore = true;

                    var merchantId = PaymentGatewayInfo.MerchantId;
                    string CRN = crn;
                    string TRN = trn;

                    // Private Key
                    rsa.FromXmlString(PaymentGatewayInfo.EncryptionKey2);

                    byte[] signMain = rsa.SignData(System.Text.Encoding.UTF8.GetBytes(merchantId + TRN + CRN), new
                    SHA1CryptoServiceProvider());

                    string Signature = Convert.ToBase64String(signMain);

                    RSACryptoServiceProvider cipher = new RSACryptoServiceProvider();
                    // Public Key
                    cipher.FromXmlString(PaymentGatewayInfo.EncryptionKey1);

                    byte[] data = Encoding.UTF8.GetBytes(amount);
                    byte[] cipherText = cipher.Encrypt(data, false);
                                        
                    data = Encoding.UTF8.GetBytes(merchantId);
                    cipherText = cipher.Encrypt(data, false);
                    string EnMID = Convert.ToBase64String(cipherText);
                                        
                    data = Encoding.UTF8.GetBytes(CRN);
                    cipherText = cipher.Encrypt(data, false);
                    string EnCRN = Convert.ToBase64String(cipherText);
                                        
                    data = Encoding.UTF8.GetBytes(TRN);
                    cipherText = cipher.Encrypt(data, false);
                    string EnTRN = Convert.ToBase64String(cipherText);
                                        
                    TransactionReference.confirmationDTO _ConfParam = new TransactionReference.confirmationDTO();
                                        
                    _ConfParam.MID = EnMID;
                    _ConfParam.CRN = EnCRN;
                    _ConfParam.TRN = EnTRN;
                    _ConfParam.SIGNATURE = Signature;
                                        
                    TransactionReference.TransactionReference _services = new TransactionReference.TransactionReference();
                    TransactionReference.saleConfResponse _TokenResponse = _services.sendConfirmation(_ConfParam);

                    if (_TokenResponse.RESCODE != null && _TokenResponse.RESCODE.ToString() != "")
                    {
                        if (_TokenResponse.RESCODE == "0" || _TokenResponse.RESCODE == "101" || _TokenResponse.RESCODE == "00")
                        {
                            response.IsSuccess = true;
                            response.TransactionNumber = trn;
                            response.Message = "Payment has been processed successfully";
                        }
                    }
                }
                else if (responseCode == "200")
                {
                    response.IsSuccess = false;
                    response.Message = "Transaction was canceled by the card owner";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to process payment";
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
