using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slidershow.Extractors
{
    public class Gelbooru : Extractor
    {
        public override List<string> GetImages(string url)
        {
            List<string> images = new List<string>();
            if(url.EndsWith(".jpg"))
            {
                images.Add(url);
            }

            return images;
        }

        public override bool Match(string url)
        {
            return url.Contains("gelbooru.com/");
        }
    }
}
