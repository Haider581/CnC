using CnC.Core.Accounts;
using CnC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CnC.Web.Controllers
{    
    public class ProxyAccountController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Authenticate()
        {
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            string username = string.Empty;
            string password = string.Empty;

            if (!headers.Contains("Authorization"))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            string credentials = headers.GetValues("Authorization").First().Trim();

            if (string.IsNullOrEmpty(credentials))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            credentials = Base64Decode(credentials).Trim(':');

            if (!credentials.Contains(':'))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            username = credentials.Split(':')[0];
            password = credentials.Split(':')[1];

            var userId = new UserService().Authenticate(username, password);
            if (userId > 0)
            {
                var proxyServer = new ProxyService().GetProxyServer(userId);
                if (proxyServer == null)
                    throw new HttpResponseException(
                        new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            ReasonPhrase = "Unable to find server"
                        });

                SettingService settingService = new SettingService();
                return Content((HttpStatusCode)200, settingService.ProxyServerSmartAppVersion + "|"
                    + settingService.ProxyServerSmartAppDownloadUrl + "|" + proxyServer.PacFile);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
