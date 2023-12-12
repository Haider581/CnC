using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CnC.Service.Lamda
{
    public class LamdaCardTransaction
    {
        [XmlElement("STAN")]
        public string STAN { get; set; }
        [XmlElement("TransactionDate")]
        public string TransactionDate { get; set; }
        [XmlElement("TransactionTime")]
        public string TransactionTime { get; set; }
        [XmlElement("TranType")]
        public string TranType { get; set; }
        [XmlElement("TranTypeDesc")]
        public string TranTypeDesc { get; set; }
        [XmlElement("ApprDenied")]
        public string ApprDenied { get; set; }
        [XmlElement("DebitCredit")]
        public string DebitCredit { get; set; }
        [XmlElement("TranCurr")]
        public string TranCurr { get; set; }
        [XmlElement("TranAmount")]
        public string TranAmount { get; set; }
        [XmlElement("AcctCurr")]
        public string AcctCurr { get; set; }
        [XmlElement("AcctCurrAmount")]
        public string AcctCurrAmount { get; set; }
        [XmlElement("AcctActivityFee")]
        public string AcctActivityFee { get; set; }
        [XmlElement("AcctProcessFee")]
        public string AcctProcessFee { get; set; }
        [XmlElement("AcctServiceFee")]
        public string AcctServiceFee { get; set; }
        [XmlElement("CATA")]
        public string CATA { get; set; }
        [XmlElement("TransactionStatus")]
        public string TransactionStatus { get; set; }
    }
}