using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammock;
using Hammock.Serialization;
using Hammock.Model;
using Hammock.Streaming;

namespace Slidershow.Downloaders
{
    public class Tumblr : Downloader
    {
        int total;
        bool searching;
        RestRequest request;
        RestClient client;

        public override string GetName()
        {
            return new Uri(url).Host.Replace("www.", "").Replace(".com", "").Replace(".tumblr", "");
        }

        public override bool Matches(string url)
        {
            return url.Contains(".tumblr.com");
        }

        public override void Search()
        {
            for (int i = 0; i < total; i += 51)
            {
                if (!searching)
                {
                    Download();
                    return;
                }

                if (i != 0)
                {
                    request.AddParameter("start", i.ToString());
                }
                var r2 = client.Request(request);
                var t2 = r2.Content.ToString().Replace("var tumblr_api_read = ", "");
                var Response = JsonParser.FromJson(t2);

                GetUrls(Response.ToDictionary(x => x.Key, x => x.Value));
            }

            Download();
        }

        public override void Stop()
        {
            if (searching)
            {
                searching = false;
            }
            else
            {
                Finish(downloads);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            searching = true;
            client = new RestClient()
            {
                Authority = url + "/api/read/json"
            };
            request = new RestRequest();
            request.AddParameter("num", "50");
            var r1 = client.Request(request);
            var t = r1.Content.ToString().Replace("var tumblr_api_read = ", "");
            var firstResponse = JsonParser.FromJson(t);
            total = Convert.ToInt32(firstResponse["posts-total"]);
        }

        private void GetUrls(Dictionary<string, object> dictionary)
        {
            var posts = dictionary.Last().Value;

            foreach (var o in posts as List<object>)
            {
                if (!searching)
                {
                    break;
                }
                var temp = o as Dictionary<string, object>;
                string imageSource = "";
                for (int i = 0; i < temp.Count; i++)
                {
                    string key = temp.Keys.ElementAt(i);
                    if (key == "url")
                    {
                        string url = temp.Values.ElementAt(i).ToString();
                        ProcessLink(url);
                    }
                    if (key == "photo-url-1280")
                    {
                        imageSource = temp.Values.ElementAt(i).ToString();
                    }
                    else if (key == "photo-url-500" && (String.IsNullOrEmpty(imageSource) || String.IsNullOrWhiteSpace(imageSource)))
                    {
                        imageSource = temp.Values.ElementAt(i).ToString();
                    }
                }

                if(!String.IsNullOrEmpty(imageSource) && !String.IsNullOrWhiteSpace(imageSource))
                {
                    Add(imageSource);
                }
            }
        }
    }
}
