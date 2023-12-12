using System;
using CnC.Web.Helper;
using System.Web.Http;
using CnC.Service;

using Newtonsoft.Json;

namespace CnC.Web.Controllers
{
    //[APIAuthorizeAttribute]
    public class TestController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        [ActionName("CardRequests")]
        public string GetCardRequestForms(DateTime? createdDateFrom = null, DateTime? createdDateTo = null)
        {
            int totalCount = 0;
            var cardRequestForms = new CardService().GetCardRequestForms(out totalCount
                                                    , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            // Serialize the object into json to remove circular refference error.
            var data = JsonConvert.SerializeObject(cardRequestForms, Formatting.Indented,
       new JsonSerializerSettings()
       {
           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
       }
   );
            return data;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        [ActionName("TopUpRequests")]
        public string GetTopUpRequests(DateTime? createdDateFrom = null, DateTime? createdDateTo = null)
        {
            //DateTime createdDateFrom = DateTime.Now.AddMonths(-6);
            //DateTime createdDateTo = DateTime.Now;
            int totalCount = 0;
            var topUpRequestForms = new TopUpService().GetTopUpRequestForms(out totalCount
                                                    , createdDateFrom: createdDateFrom, createdDateTo: createdDateTo);

            var data = JsonConvert.SerializeObject(topUpRequestForms, Formatting.Indented,
       new JsonSerializerSettings()
       {
           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
       }
   );
            return data;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        [ActionName("CardRequestAudit")]
        public string AuditCardRequests()
        {
            int totalCount = 0;
            var cardRequestAudit = new AdminService().GetCardRequestAudit(out totalCount);

            var data = JsonConvert.SerializeObject(cardRequestAudit, Formatting.Indented,
      new JsonSerializerSettings()
      {
          ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
      }
  );
            return data;
        }

    }
}
