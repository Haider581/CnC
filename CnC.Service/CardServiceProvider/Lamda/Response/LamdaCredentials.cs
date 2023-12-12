using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CnC.Service.Lamda
{
    public class LamdaCredentials
    {
        [XmlElement("MerchantId")]
        public string MerchantId { get; set; }
        [XmlElement("TerminalId")]
        public string TerminalId { get; set; }
    }
}