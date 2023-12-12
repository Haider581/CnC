using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CnC.Service.Lamda
{
    public class LamdaCard
    {
        [XmlElement("CardId")]
        public string CardId { get; set; }
        [XmlElement("EmbossName")]
        public string EmbossName { get; set; }
        [XmlElement("CardNumber")]
        public string CardNumber { get; set; }
        [XmlElement("OrderStatus")]
        public string OrderStatus { get; set; }
        [XmlElement("ExpirationDate")]
        public string ExpirationDate { get; set; }
        [XmlElement("Type")]
        public string Type { get; set; }
        [XmlElement("Product")]
        public string Product { get; set; }
        [XmlElement("Status")]
        public string Status { get; set; }
        [XmlElement("IssuedDate")]
        public string IssuedDate { get; set; }
        [XmlElement("IssuedTime")]
        public string IssuedTime { get; set; }
        [XmlElement("IssuedBy")]
        public string IssuedBy { get; set; }
        [XmlElement("AmountLimit")]
        public string AmountLimit { get; set; }
        [XmlElement("Currency")]
        public string Currency { get; set; }
    }
}