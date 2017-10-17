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

        public override bool IsSearching()
        {
            return searching;
        }

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
                for (int i = 0; i < temp.Count; i++)
                {
                    string key = temp.Keys.ElementAt(i);
                    string value = temp.Values.ElementAt(i)?.ToString();
                    if (key == "url")
                    {
                        ProcessLink(value);
                    }
                    else if(key == "photo-caption")
                    {
                        List<string> links = FindLinks(value);
                        if (links.Count == 0)
                        {
                            links.Add(url);
                        }

                        for (int d = 0; d < links.Count; d++)
                        {
                            if (!searching)
                            {
                                Download();
                                return;
                            }

                            ProcessLink(links[d]);
                        }
                    }
                    else if(key == "photos")
                    {
                        List<object> links = (List<object>)temp.Values.ElementAt(i);
                        for(int l = 0; l < links.Count;l++)
                        {
                            Dictionary<string, object> dict = (Dictionary<string, object>)links[l];
                            for (int d = 0; d < dict.Count; d++)
                            {
                                string photoKey = dict.ElementAt(d).Key;
                                string photoUrl = dict.ElementAt(d).Value.ToString();

                                if(photoKey == "photo-url-1280" || photoKey == "photo-url-500" || photoKey == "photo-url-400" || photoKey == "photo-url-250" || photoKey == "photo-url-100" || photoKey == "photo-url-75")
                                {
                                    Add(photoUrl);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
