using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CnC.Service.Lamda
{
    public class LamdaCardInfoLCS
    {
        [XmlElement("xsactv")]
        public string XsActv { get; set; }
        [XmlElement("xsstat")]
        public string XsStat { get; set; }
        [XmlElement("xsAdditionalAmountAcctType1")]
        public string XsAdditionalAmountAcctType1 { get; set; }
        [XmlElement("xsAdditionalAmountType1")]
        public string XsAdditionalAmountType1 { get; set; }
        [XmlElement("xsAdditionalAmountCurrency1")]
        public string XsAdditionalAmountCurrency1 { get; set; }
        [XmlElement("xsAdditionalAmount1")]
        public string XsAdditionalAmount1 { get; set; }
        [XmlElement("xsAdditionalAmountAcctType2")]
        public string XsAdditionalAmountAcctType2 { get; set; }
        [XmlElement("xsAdditionalAmountType2")]
        public string XsAdditionalAmountType2 { get; set; }
        [XmlElement("xsAdditionalAmountCurrency2")]
        public string XsAdditionalAmountCurrency2 { get; set; }
        [XmlElement("xsAdditionalAmount2")]
        public string XsAdditionalAmount2 { get; set; }
    }
}