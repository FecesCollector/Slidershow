using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Slidershow.Extractors;

namespace Slidershow
{
    public abstract class Downloader
    {
        public HtmlDocument mainDocument;
        public string url;
        public string name;

        public delegate void OnFinish(int downloads);
        public OnFinish onFinish;

        public abstract bool Matches(string url);
        public abstract void Search();
        public abstract void Stop();
        public abstract bool IsSearching();

        List<string> pendingFiles = new List<string>();
        public int downloads;
        
        public void Create(string url)
        {
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            url = Uri.UnescapeDataString(url);

            mainDocument = GetDocument(url);
            this.url = url;
            name = GetName();

            Console.WriteLine("Created downloader (" + url + ")");
            Initialize();
        }

        public virtual string GetName()
        {
            string newName = mainDocument.DocumentNode.SelectSingleNode("//head/title").InnerText;

            newName = newName.Replace("/", "-");
            newName = newName.Replace(".", "-");
            newName = newName.Replace("?", "");
            newName = newName.Replace(":", "");
            newName = newName.Replace("|", "-");
            newName = newName.Replace("\\", "-");

            return newName;
        }

        protected virtual void Initialize()
        {

        }

        public void ProcessLink(string url)
        {
            Console.WriteLine("Process link: " + url);
            for(int i = 0;i < Program.extratorTypes.Count - 1;i++)
            {
                bool match = Program.extratorTypes[i].Match(url);
                if (match)
                {
                    List<string> links = Program.extratorTypes[i].GetImages(url);
                    if(links != null)
                    {
                        for (int m = 0; m < links.Count; m++)
                        {
                            string ext = Path.GetExtension(links[m]);
                            if (ext != ".com")
                            {
                                Add(links[m]);
                            }
                        }
                    }
                }
            }
        }

        public void Add(string url)
        {
            pendingFiles.Add(url);
            string friendlyName = Path.GetFileName(url);
            Console.WriteLine("Found: " + pendingFiles.Count + ", " + friendlyName);
        }

        public int Files
        {
            get
            {
                return pendingFiles.Count;
            }
        }

        public async void Download()
        {
            Program.Check();
            await Task.Delay(0);
            
            Console.WriteLine("Download starting");

            for (int i = 0; i < pendingFiles.Count;i++)
            {
                CustomWebClient webClient = new CustomWebClient();

                string fileName = "Galleries/" + name + "/" + Path.GetFileName(pendingFiles[i]);
                if (!File.Exists(fileName) && !(pendingFiles[i] == "" || pendingFiles[i] == null || pendingFiles[i] == string.Empty))
                {
                    webClient.DownloadFileAsync(new Uri(pendingFiles[i]), fileName);
                    webClient.DownloadFileCompleted += DownloadCompleted;
                }
                else
                {
                    NewDownload();
                }
            }
        }

        void NewDownload()
        {
            downloads++;
            Console.WriteLine("Downloaded: " + downloads + " / " + pendingFiles.Count);

            if (downloads == pendingFiles.Count)
            {
                Finish(downloads);
            }
        }

        private void DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            NewDownload();
        }

        public void Finish(int downloads)
        {
            onFinish?.Invoke(downloads);
        }

        public static List<string> FindLinks(HtmlDocument document)
        {
            List<string> linksFound = new List<string>();
            HtmlNode node = document.DocumentNode;
            HtmlNodeCollection nodes = node.SelectNodes("//a[@href]");

            if (nodes == null) return linksFound;

            for (int i = 0; i < nodes.Count;i++)
            {
                HtmlAttribute att = nodes[i].Attributes["href"];
                string link = Uri.UnescapeDataString(att.Value);
                if(link.Contains("&") && link.Contains("t.umblr.com/redirect"))
                {
                    //case for tumblr redirect links
                    link = link.Replace("http://t.umblr.com/redirect?z=", "");
                    int ampIndex = link.IndexOf('&');
                    link = link.Substring(0, ampIndex);
                }

                linksFound.Add(link);
            }

            return linksFound;
        }

        public static List<string> FindLinks(string text)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(text);

            return FindLinks(document);
        }

        public static List<string> FindImages(string url)
        {
            return FindContent(url, "img", "src");
        }

        public static List<string> FindImages(HtmlDocument document)
        {
            return FindContent(document, "img", "src");
        }

        public static List<string> FindContent(string url, string content, string attribute)
        {
            return FindContent(GetDocument(url), content, attribute);
        }

        public static List<string> FindContent(HtmlDocument document, string tag, string attribute)
        {
            List<string> linksFound = new List<string>();
            if (document == null) return linksFound;

            var urls = document.DocumentNode.Descendants(tag).Select(e => e.GetAttributeValue(attribute, null)).Where(s => !String.IsNullOrEmpty(s)).ToList();

            for (int i = 0; i < urls.Count; i++)
            {
                linksFound.Add(Uri.UnescapeDataString(urls[i]));
            }

            return linksFound;
        }

        public static async Task<string> GetSource(string url)
        {
            return await new WebClient().DownloadStringTaskAsync(url);
        }

        public static HtmlDocument GetDocument(string url)
        {
            return GetDocument(new Uri(url));
        }

        public static HtmlDocument GetDocument(Uri url)
        {
            CookieContainer cookies = new CookieContainer();
            cookies.Add(new Cookie("nw", "1", "/", "e-hentai.org")); //g-ehentai
            cookies.Add(new Cookie("over18", "1", "/", ".reddit.com")); //reddit

            CustomWebClient client = new CustomWebClient()
            {
                CookieContainer = cookies
            };
            HtmlDocument document = new HtmlDocument();

            try
            {
                document.LoadHtml(client.DownloadString(url));
                return document;
            }
            catch
            {
                return null;
            }
        }
    }
}
