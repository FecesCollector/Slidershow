using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slidershow.Extractors
{
    public class Mixtape : Extractor
    {
        public override List<string> GetImages(string url)
        {
            List<string> images = new List<string>();

            images.Add(url);

            return images;
        }

        public override bool Match(string url)
        {
            return url.Contains("my.mixtape.moe") && (url.Contains(".mp4") || url.Contains(".webm"));
        }
    }
}
