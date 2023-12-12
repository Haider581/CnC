using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CnC.Service.Lamda
{
    public class ILamdaResponse : ILamdaCustomer
    {
        [XmlElement("Credentials")]
        public LamdaCredentials Credentials { get; set; }        
        [XmlElement("TransactionId")]
        public string TransactionId { get; set; }
        [XmlElement("TransactionType")]
        public string TransactionType { get; set; }
        [XmlElement("ClientId")]
        public string ClientId { get; set; }
        [XmlElement("ResponseCode")]
        public string ResponseCode { get; set; }
        [XmlElement("ResponseTxt")]
        public string ResponseTxt { get; set; }
    }
}