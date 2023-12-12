using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CnC.Core.Cards;
using CnC.Core.Customers;
using System.Net.Http;

namespace CnC.Service.Lamda
{
    public class LamdaCardServiceProvider : ICardServiceProvider
    {
        /// <summary>
        /// Return Card Balance and Payment History
        /// </summary>
        /// <param name="clientId">LAMDA Provided Client Id</param>
        /// <param name="cardId">LAMDA Provided Card Id</param>
        /// <param name="expiryDate">Card Expiry Date (YYMM)</param>
        /// <param name="dateOfBirth">Client Date Of Birth (YYYYMMDD)</param>
        /// <param name="maxRows">Number of records to return</param>
        /// <param name="startDate">1 for 21st Century and 0 for 20th (YYMMDD)</param>
        /// <param name="endDate">1 for 21st Century and 0 for 20th (YYMMDD)</param>
        /// <returns></returns>
        public Card GetCardInfo(string clientId, string cardId, string expiryDate, string dateOfBirth
            , int maxRows, string startDate, string endDate)
        {
            using (var client = new HttpClient())
            {
                //string phpUrl = "http://localhost:8080/lamda/";
                //string merchantId = "100000103";
                //string terminalId = "10000126";
                //string terminalPassword = "Y1Y2NeAz";

                //clientId = "67420";
                //cardId = "52715";
                //expiryDate = "2001";
                //dateOfBirth = "19850101";
                //maxRows = 10;
                //startDate = "1170117";
                //endDate = "1170628";

                string phpUrl = "";
                string merchantId = "";
                string terminalId = "";
                string terminalPassword = "";

                var settingService = new SettingService();

                if (settingService.LamdaTestMode)
                {
                    phpUrl = settingService.LamdaPhpUrlTest;
                    merchantId = settingService.LamdaMerchantIdTest;
                    terminalId = settingService.LamdaTerminalIdTest;
                    terminalPassword = settingService.LamdaTerminalPasswordTest;
                }
                else
                {
                    phpUrl = settingService.LamdaPhpUrl;
                    merchantId = settingService.LamdaMerchantId;
                    terminalId = settingService.LamdaTerminalId;
                    terminalPassword = settingService.LamdaTerminalPassword;
                }

                if (!phpUrl.Contains("http://") && !phpUrl.Contains("https://"))
                    phpUrl += "http://";
                string apiUrl = string.Format(
                phpUrl + @"Lamda_GetCardInfo.php?merchantid={0}&terminalid={1}&terminalpassword={2}" +
                "&clientid={3}&cardid={4}&expdate={5}&maxrows={6}&startdate={7}&enddate={8}&dob={9}",
                merchantId, terminalId, terminalPassword, clientId, cardId, expiryDate, "",
                startDate, endDate, dateOfBirth);

                var response = client.GetStringAsync(apiUrl);
                var xmlResult = response.Result;

                if (response.IsCompleted && !string.IsNullOrEmpty(xmlResult))
                {
                    LamdaCardInfoResponse lamdaCard = new CommonService().FromXml<LamdaCardInfoResponse>(xmlResult);

                    if (lamdaCard != null)
                    {
                        var card = new Card();
                        card.Balance = Convert.ToDecimal(lamdaCard.CardInfoLCS.XsAdditionalAmount1) / 100;
                        card.PaymentTransactions = new List<CardPaymentTransaction>();

                        foreach (LamdaCardTransaction transaction in lamdaCard.CardTransactions)
                        {
                            card.PaymentTransactions.Add(new CardPaymentTransaction()
                            {
                                Amount = Convert.ToDecimal(transaction.TranAmount) / 100,
                                AccountServiceFee = Convert.ToDecimal(transaction.AcctServiceFee) / 100,
                                CreatedOn = DateTime.ParseExact(transaction.TransactionDate + " " 
                                + transaction.TransactionTime
                                , "yyyyMMdd HHmmss", null),
                                Description = transaction.CATA,
                                TransactionTypeDescription = transaction.TranTypeDesc,
                                IsApproved = transaction.ApprDenied == "A" ? true : false,
                                IsDebit = transaction.DebitCredit == "D" ? true : false,
                                Status = transaction.TransactionStatus,
                                AccountCurrencyAmount = Convert.ToDecimal(transaction.AcctCurrAmount) / 100
                            });
                        }

                        return card;
                    }
                }
            }
            return null;
        }

        public List<Card> GetCards(int clientId)
        {
            using (var client = new HttpClient())
            {
                var settingService = new SettingService();
                string phpUrl = settingService.LamdaPhpUrl;
                string merchantId = settingService.LamdaMerchantId;
                string terminalId = settingService.LamdaTerminalId;
                string terminalPassword = settingService.LamdaTerminalPassword;

                string apiUrl = string.Format(
                    phpUrl + @"Lamda_GetAccountCards.php?merchantid={0}&terminalid={1}
&terminalpassword={2}&clientid={3}", merchantId, terminalId, terminalPassword, clientId.ToString());

                var response = client.GetStringAsync(apiUrl);
                var xmlResult = response.Result;

                if (response.IsCompleted && !string.IsNullOrEmpty(xmlResult))
                {
                    LamdaAccountCardListResponse cardList = new CommonService().FromXml<LamdaAccountCardListResponse>(xmlResult);

                    if (cardList != null && cardList.Cards.Count() > 0)
                    {
                        var cards = new List<Card>();

                        foreach (LamdaCard card in cardList.Cards)
                        {
                        }
                    }

                    return null;
                }
            }
            return null;
        }

        public int NewCardRequest(Customer customer, CardRequest cardRequest)
        {
            throw new NotImplementedException();
        }
    }
}
