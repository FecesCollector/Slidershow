using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slidershow.Extractors
{
    public class Tumblr : Extractor
    {
        public override List<string> GetImages(string url)
        {
            List<string> images = new List<string>();

            List<string> video = FindContent(GetDocument(url), "iframe", "src");

            if(video.Count > 0 && video[0].Contains("/video/"))
            {
                List<string> files = FindContent(GetDocument(video[0]), "source", "src");
                images.Add(files[0]+".mp4");
            }

            return images;
        }

        public override bool Match(string url)
        {
            return url.Contains(".tumblr.com/post/");
        }
    }
}
