using CnC.Core;
using CnC.Core.Cards;
using CnC.Core.Payments;
using CnC.Core.TopUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CnC.Web.Models
{
    public class CustomerNewRequestContainer
    {
        public RequestForm RequestFormObj { get; set; }
        public List<CardRequest> CardRequestCollection { get; set; }
        public List<TopUpRequest> TopUpRequestCollection { get; set; }
        public List<Payment> Payments { get; set; }
        public string ProofOfAddressType { get; set; }
        public List<string> ListOfProofOfAddrss { get; set; }
        public string SourcceOfFundType { get; set; }
        public List<string> ListOfSourceOfFundsDocs { get; set; }

        public string CardQty { get; set; }
        public string CardTypeIds { get; set; }

        public string Message { get; set; }
    }
}