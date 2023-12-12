using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CnC.Service.Lamda
{
    [XmlRoot("CardInfoResponse")]
    public class LamdaCardInfoResponse : ILamdaResponse
    {
        [XmlElement("CardInfoLCS")]
        public LamdaCardInfoLCS CardInfoLCS { get; set; }
        [XmlArray("TranRangeSummary"), XmlArrayItem("Transaction")]
        public LamdaCardTransaction[] CardTransactions { get; set; }
    }
}