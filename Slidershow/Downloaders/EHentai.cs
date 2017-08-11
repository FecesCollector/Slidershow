using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slidershow.Downloaders
{
    public class EHentai : Downloader
    {
        int total;
        int pages;
        bool searching;

        public override bool Matches(string url)
        {
            return url.StartsWith("https://e-hentai.org/g/");
        }

        protected override void Initialize()
        {
            base.Initialize();

            searching = true;

            var findclasses = mainDocument.DocumentNode
              .Descendants("td")
              .Where(d =>
                 d.Attributes.Contains("class")
                 &&
                 d.Attributes["class"].Value.Contains("gdt2")
              ).ToList();

            for (int i = 0; i < findclasses.Count; i++)
            {
                if (findclasses[i].InnerText.Contains("pages"))
                {
                    string pageNumbers = findclasses[i].InnerText.Replace(" ", "");
                    pageNumbers = pageNumbers.Replace("pages", "");

                    total = int.Parse(pageNumbers);
                    pages = (int)Math.Ceiling(total / 40f);
                }
            }
        }

        public override async void Search()
        {
            await Task.Delay(0);
            
            for (int p = 0; p < pages; p++)
            {
                string pageUrl = url + "?p=" + p;

                if(p != 0)
                {
                    mainDocument = GetDocument(pageUrl);
                }
                var links = FindLinks(mainDocument);
                
                for (int i = 0; i < links.Count; i++)
                {
                    if(links[i].Contains("/s/"))
                    {
                        var smallerLinks = FindImages(links[i]);
                        for (int s = 0; s < smallerLinks.Count; s++)
                        {
                            if (smallerLinks[s].Contains("/h/"))
                            {
                                string link = smallerLinks[s];
                                Add(link);

                                Console.Clear();
                                Console.WriteLine("Found: " + Files + " / " + total);
                                if (!searching)
                                {
                                    Download();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            Download();
        }

        public override void Stop()
        {
            if(searching)
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
