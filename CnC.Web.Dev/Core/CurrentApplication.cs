using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CnC.Web.Core
{
    public class CurrentApplication
    {
        public static CurrentApplication Instance
        {
            get
            {
                return HttpContext.Current.Application["CurrentApplication"] as CurrentApplication;
            }

        }
    }
}