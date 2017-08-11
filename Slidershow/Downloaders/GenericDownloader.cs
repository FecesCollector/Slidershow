using System.Collections.Generic;
using System;

namespace Slidershow.Downloaders
{
    public class GenericDownloader : Downloader
    {
        bool searching = true;

        public override bool Matches(string url)
        {
            return true;
        }

        public override string GetName()
        {
            return new Uri(url).Host.Replace("www.", "").Replace(".com", "");
        }

        public override void Search()
        {
            List<string> links = FindLinks(mainDocument);
            if(links.Count == 0)
            {
                links.Add(url);
            }

            for (int i = 0; i < links.Count;i++)
            {
                if(!searching)
                {
                    Download();
                    return;
                }

                ProcessLink(links[i]);
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
    }
}
