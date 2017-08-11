using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammock;
using Hammock.Web;
using System.IO;
using Hammock.Serialization;
using Hammock.Model;
using Hammock.Streaming;

namespace Slidershow.Extractors
{
    public class Imgur : Extractor
    {
        public override bool Match(string url)
        {
            return url.Contains("imgur.com") && !url.Contains("domain");
        }

        public override List<string> GetImages(string url)
        {
            List<string> images = new List<string>();

            bool isAlbum = url.Contains("/a/");
            bool isDirect = url.Contains("i.imgur.com");
            RestClient client = new RestClient();
            if (isDirect)
            {
                images.Add(url);
                return images;
            }
            else
            {
                string id = Path.GetFileName(url);

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
            request.AddHeader("Authorization", "Client-ID b13daf267e945c6");

            var response = JsonParser.FromJson(client.Request(request).Content);

            if (isAlbum)
            {
                List<object> data = (List<object>)response["data"];
                for (int i = 0; i < data.Count; i++)
                {
                    Dictionary<string, object> list = (Dictionary<string, object>)(data[i]);
                    for (int m = 0; m < list.Count; m++)
                    {
                        string line = list.ElementAt(m).ToString();
                        if (line.StartsWith("[link, http://i.imgur.com/"))
                        {
                            string image = line.Substring(7);
                            image = image.Substring(0, image.Length - 1);
                            images.Add(image);
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
                    if (line.StartsWith("[link, http://i.imgur.com/"))
                    {
                        string image = line.Substring(7);
                        image = image.Substring(0, image.Length - 1);
                        images.Add(image);
                    }
                }
            }

            return images;
        }
    }
}
