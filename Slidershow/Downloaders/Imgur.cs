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
    public class Imgur : Downloader
    {
        bool searching;

        public override bool IsSearching()
        {
            return searching;
        }

        public override string GetName()
        {
            return System.IO.Path.GetFileNameWithoutExtension(url);
        }

        public override bool Matches(string url)
        {
            return url.Contains("imgur.com/");
        }

        public override void Search()
        {
            bool isAlbum = url.Contains("/a/");
            bool isDirect = url.Contains("i.imgur.com");
            RestClient client = new RestClient();
            if (isDirect)
            {
                Add(url);

                if (!searching)
                {
                    Download();
                    return;
                }
            }
            else
            {
                string id = GetName();
                if (isAlbum)
                {
                    client.Authority = "https://api.imgur.com/3/album/" + id + "/images";
                }
                else
                {
                    client.Authority = "https://api.imgur.com/3/image/" + id;
                }
            }

            RestRequest request = new RestRequest();
            request.AddHeader("authorization", "Client-ID b13daf267e945c6");
            string json = client.Request(request).Content;
            var response = JsonParser.FromJson(json);

            if (isAlbum)
            {
                List<object> data = (List<object>)response["data"];
                for (int i = 0; i < data.Count; i++)
                {
                    Dictionary<string, object> list = (Dictionary<string, object>)(data[i]);
                    for (int m = 0; m < list.Count; m++)
                    {
                        string line = list.ElementAt(m).ToString();
                        if (line.StartsWith("[link, http://i.imgur.com/") || line.StartsWith("[link, https://i.imgur.com/"))
                        {
                            string image = line.Substring(7);
                            image = image.Substring(0, image.Length - 1);
                            Add(image);

                            if (!searching)
                            {
                                Download();
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                Dictionary<string, object> list = (Dictionary<string, object>)(response["data"]);
                for (int m = 0; m < list.Count; m++)
                {
                    string line = list.ElementAt(m).ToString();
                    if (line.StartsWith("[link, http://i.imgur.com/") || line.StartsWith("[link, https://i.imgur.com/"))
                    {
                        string image = line.Substring(7);
                        image = image.Substring(0, image.Length - 1);
                        Add(image);

                        if (!searching)
                        {
                            Download();
                            return;
                        }
                    }
                }
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
        }
    }
}
