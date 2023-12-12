using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core.Affiliates
{
    public class Commission : CnCObject
    {        
        public int AffiliateId { get; set; }
        public decimal SalesValue { get; set; }
        public decimal Rate { get; set; }
        public decimal Commision { get; set; }
        public decimal Bonus { get; set; }
        
    }
}
