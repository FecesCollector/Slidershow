using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slidershow.Extractors
{
    public class GenericExtractor : Extractor
    {
        public override List<string> GetImages(string url)
        {
            List<string> list = new List<string>();
            list.Add(url);
            return list;
        }

        public override bool Match(string url)
        {
            return true;
        }
    }
}
