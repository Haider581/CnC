using CnC.Core.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CnC.Web.Core
{
    public class CurrentSession
    {
        private static CurrentSession currentSession = null;
        public User LoginUser { get; set; }
        public static CurrentSession Instance
        {
            get
            {
                currentSession = HttpContext.Current.Session["CurrentSession"] as CurrentSession;
                //if (currentSession == null)
                //{
                //    currentSession = new CurrentSession();
                //}                
                
                return currentSession;
            }
        }



    }
}