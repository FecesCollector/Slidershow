using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slidershow.Extractors
{
    public class WebmShare : Extractor
    {
        public override List<string> GetImages(string url)
        {
            List<string> images = new List<string>();
            List<string> content = FindContent(GetDocument(url), "meta", "content");
            Console.WriteLine(content.Count);

            bool isWebm = content.Contains("video/webm");
            for (int i = 0; i < content.Count; i++)
            {
                if (isWebm && content[i].EndsWith(".webm"))
                {
                    images.Add("http:" + content[i]);

                    Console.Clear();
                    Console.WriteLine("Found: " + content[i]);
                }
                else if (!isWebm && content[i].EndsWith(".mp4"))
                {
                    images.Add("http:" + content[i]);
                    Console.Clear();

                    Console.WriteLine("Found: " + content[i]);
                }
            }

            return images;
        }

        public override bool Match(string url)
        {
            return url.Contains("webmshare.com");
        }
    }
}
