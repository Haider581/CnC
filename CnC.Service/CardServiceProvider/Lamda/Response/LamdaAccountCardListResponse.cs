using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CnC.Service.Lamda
{
    [XmlRoot("AccountCardListResponse")]
    public class LamdaAccountCardListResponse : ILamdaResponse
    {
        [XmlElement("CardListCounter")]
        public string CardListCounter { get; set; }
        [XmlArray("CardList"), XmlArrayItem("Card")]
        public LamdaCard[] Cards { get; set; }
    }
}