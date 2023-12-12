using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CnC.Service.Lamda
{
    public class ILamdaCustomer
    {
        [XmlElement("Firstname")]
        public string Firstname { get; set; }
        [XmlElement("Lastname")]
        public string Lastname { get; set; }
        [XmlElement("Username")]
        public string Username { get; set; }
        [XmlElement("PrimaryEmail")]
        public string PrimaryEmail { get; set; }
        [XmlElement("CustomerStatus")]
        public string CustomerStatus { get; set; }
    }
}