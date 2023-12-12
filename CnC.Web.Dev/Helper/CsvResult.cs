using CnC.Core.Accounts;
using CnC.Core.TopUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CnC.Web.Helper
{
    public class CsvResult: ActionResult
    {
        private readonly IList<TopUpRequestForm> _records;
        private readonly string _filename;
        public CsvResult(IList<TopUpRequestForm> records, string filename)
        {
            _records = records;
            _filename = filename;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var sb = new StringBuilder();
            sb.Append("Card" + "," + "Amount" + ", " + "Currency" + ", " + "Accountid");
            sb.AppendLine();
            foreach (var topupRequestForm in _records)
            {
              
                foreach (var topUpRequest in topupRequestForm.TopUpRequests)
                {
                    string cardNUmber = topUpRequest.Card.Number;
              
                    string lastChr= cardNUmber.Substring(Math.Max(0, cardNUmber.Length - 4));
                   
                    sb.Append(lastChr + "," + topUpRequest.Amount + "," + "EUR" + ", " + topupRequestForm.Payments[0].TransactionAccountNo);
                    sb.AppendLine();
                }
                
               
            }
            
            var response = context.HttpContext.Response;
            response.ContentType = "text/csv";
            response.AddHeader("content-disposition",
                String.Format("attachment; filename={0}", _filename));

            response.Write(sb.ToString());

            response.Flush();
            response.End();
        }
    }
}