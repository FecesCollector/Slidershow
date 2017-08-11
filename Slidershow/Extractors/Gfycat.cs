using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Slidershow.Extractors
{
    public class Gfycat : Extractor
    {
        public override List<string> GetImages(string url)
        {
            List<string> content = FindContent(GetDocument(url), "source", "src");
            List<string> images = new List<string>();

            for (int c = 0; c < content.Count; c++)
            {
                string ext = Path.GetExtension(content[c]);
                if (ext == ".mp4")
                {
                    if (!images.Contains(content[c]))
                    {
                        images.Add(content[c]);
                    }
                }
            }

            return images;
        }

        public override bool Match(string url)
        {
            return url.StartsWith("https://gfycat.com/");
        }
    }
}
