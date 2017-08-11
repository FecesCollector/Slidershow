using System;
using System.Net;

namespace Slidershow
{
    public class CustomWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; set; }

        public CustomWebClient() : base()
        {
            CookieContainer = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            HttpWebRequest webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = CookieContainer;
            }

            return request;
        }
    }
}
