using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Slidershow
{
    public abstract class Extractor
    {
        public abstract bool Match(string url);
        public abstract List<string> GetImages(string url);

        public async Task<string> GetSource(string url)
        {
            return await Downloader.GetSource(url);
        }

        public HtmlDocument GetDocument(string url)
        {
            return Downloader.GetDocument(url);
        }

        public List<string> FindContent(HtmlDocument p, string v1, string v2)
        {
            return Downloader.FindContent(p, v1, v2);
        }
    }
}
