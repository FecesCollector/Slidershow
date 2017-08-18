using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xaml;
using Slidershow.Downloaders;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Threading;

namespace Slidershow
{
    public class Program
    {
        static Downloader downloader;

        public static ImageForm imageForm;
        public static Gallery current;
        public static List<Extractor> extratorTypes = new List<Extractor>();
        public static KeyIntercept interceptor;
        
        public static bool IsDownloading
        {
            get
            {
                return downloader != null;
            }
        }

        static ManualResetEvent quitEvent = new ManualResetEvent(false);

        [STAThread]
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            var extrators = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Extractor))).ToList();
            for (int i = 0; i < extrators.Count; i++)
            {
                Extractor extractor = (Extractor)Activator.CreateInstance(extrators[i]);
                if (extrators[i] != typeof(Extractors.GenericExtractor))
                {
                    extratorTypes.Add(extractor);
                }
            }

            extratorTypes.Add(Activator.CreateInstance<Extractors.GenericExtractor>());


            Console.WriteLine("Slidershow Application Launched");
            while (true)
            {
                string command = Console.ReadLine();
                Run(command);
            }
        }

        public static void Press(Keys key)
        {
            if (IsDownloading)
            {
                if (key == Keys.Escape)
                {
                    CancelDownload();
                }
                return;
            }

            if (imageForm != null)
            {
                imageForm.Press(key);
            }
        }

        static bool Run(string command)
        {
            string[] parameters = command.ToLower().Split(' ');
            if (!command.Contains(' '))
            {
                parameters = new string[] { command.ToLower() };
            }

            if (parameters.Length > 0)
            {
                if (parameters.Length > 1)
                {
                    string rest = command.Substring(parameters[0].Length + 1);
                    if (parameters[0] == "get")
                    {
                        Check();
                        if (parameters.Length == 3)
                        {
                            Get(parameters[1], parameters[2] == "-g");
                        }
                        else
                        {
                            Get(parameters[1], false);
                        }

                        return true;
                    }
                    else if (parameters[0] == "hide" || parameters[0] == "unhide")
                    {
                        Check();
                        if (parameters[1] == "all")
                        {
                            Hide("Galleries/", parameters[0] == "hide");
                        }
                        else
                        {
                            List<string> galleries = Directory.GetDirectories("Galleries").ToList();
                            if (int.TryParse(parameters[1], out int galleryId))
                            {
                                if (galleryId >= 0 && galleryId < galleries.Count)
                                {
                                    Hide(galleries[galleryId], parameters[0] == "hide");
                                }
                                else
                                {
                                    Console.WriteLine("Error: Out of range");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error: Second parameter isnt a valid number (" + parameters[1] + ")");
                            }
                        }
                    }
                    else if (parameters[0] == "convert")
                    {
                        Check();
                        if (parameters[1] == "all")
                        {
                            Convert("Galleries/");
                        }
                        else
                        {
                            List<string> galleries = Directory.GetDirectories("Galleries").ToList();
                            if (int.TryParse(parameters[1], out int galleryId))
                            {
                                if (galleryId >= 0 && galleryId < galleries.Count)
                                {
                                    Convert(galleries[galleryId]);
                                }
                                else
                                {
                                    Console.WriteLine("Error: Out of range");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error: Second parameter isnt a valid number (" + parameters[1] + ")");
                            }
                        }
                    }
                    else if (parameters[0] == "load")
                    {
                        Check();
                        List<string> galleries = Directory.GetDirectories("Galleries").ToList();
                        if (int.TryParse(parameters[1], out int galleryId))
                        {
                            if (galleryId >= 0 && galleryId < galleries.Count)
                            {
                                Console.WriteLine("Loading gallery: " + Path.GetFileNameWithoutExtension(galleries[galleryId]));
                                CreateSlideshow(galleries[galleryId]);
                            }
                            else
                            {
                                Console.WriteLine("Error: Out of range");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Second parameter isnt a valid number (" + parameters[1] + ")");
                        }
                    }
                    else if (parameters[0] == "delete")
                    {
                        Check();
                        if (parameters[1] == "all")
                        {
                            List<string> galleries = Directory.GetDirectories("Galleries").ToList();
                            for(int i = 0; i < galleries.Count;i++)
                            {
                                Directory.Delete(galleries[i], true);
                                Console.WriteLine("Deleted " + Path.GetFileNameWithoutExtension(galleries[i]));
                            }
                        }
                        else
                        {
                            List<string> galleries = Directory.GetDirectories("Galleries").ToList();
                            if (int.TryParse(parameters[1], out int galleryId))
                            {
                                if (galleryId >= 0 && galleryId < galleries.Count)
                                {
                                    if (Directory.Exists(galleries[galleryId]))
                                    {
                                        Directory.Delete(galleries[galleryId], true);
                                        Console.WriteLine("Deleted " + Path.GetFileNameWithoutExtension(galleries[galleryId]));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error: Out of range");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error: Second parameter isnt a valid number (" + parameters[1] + ")");
                            }
                        }
                    }
                }
                else if (parameters[0] == "open")
                {
                    Process.Start("Galleries");
                }
                else if(parameters[0] == "clear")
                {
                    Console.Clear();
                }
                else if (parameters[0] == "list")
                {
                    List<string> galleries = Directory.GetDirectories("Galleries").ToList();
                    for (int i = 0; i < galleries.Count; i++)
                    {
                        string title = Path.GetFileName(galleries[i]);
                        int length = title.Length > 32 ? 32 : title.Length;
                        title = title.Substring(0, length) + "...";

                        int files = Gallery.ValidFiles(Directory.GetFiles(galleries[i]));

                        Console.WriteLine(i.ToString("000") + " " + title + " " + files + " files");
                    }
                }
                else if(parameters[0] == "exit" || parameters[0] == "quit")
                {
                    Environment.Exit(0);
                }
            }

            return false;
        }

        public static void CancelDownload()
        {
            if(IsDownloading)
            {
                if(downloader.IsSearching())
                {
                    Console.WriteLine("Search cancelled");
                }
                else
                {
                    Console.WriteLine("Download cancelled");
                }
                downloader.Stop();
            }
        }
        
        public static void Get(string url, bool useGeneric)
        {
            interceptor = new KeyIntercept();
            
            if (!useGeneric)
            {
                var downloaders = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Downloader))).ToList();
                for (int i = 0; i < downloaders.Count; i++)
                {
                    if (downloaders[i] != typeof(GenericDownloader))
                    {
                        downloader = (Downloader)Activator.CreateInstance(downloaders[i]);
                        if (downloader.Matches(url))
                        {
                            downloader.onFinish += OnFinish;
                            Console.WriteLine("Using " + downloader.GetType().Name + " Downloader");

                            downloader.Create(url);
                            downloader.Search();

                            return;
                        }
                    }
                }
            }

            downloader = new GenericDownloader();
            downloader.onFinish += OnFinish;
            Console.WriteLine("No suitable downloader found, using GenericDownloader");

            downloader.Create(url);
            downloader.Search();
        }
        
        static void OnFinish(int downloads)
        {
             Console.WriteLine("Finished downloading " + (IsDownloading ? downloader.name : "") + " (" + downloads + ")");

            downloader = null;
            if (interceptor != null)
            {
                interceptor.Die();
                interceptor = null;
            }
            GC.Collect();
        }

        public static void LoadImage()
        {
            if(imageForm != null)
            {
                imageForm.SetImage(current.Current);
            }
        }

        public static void CreateSlideshow(string path)
        {
            interceptor = new KeyIntercept();
            current = new Gallery(path);

            imageForm = new ImageForm();
            LoadImage();
            imageForm.ShowDialog();
        }

        public static void Check()
        {
            if(!Directory.Exists("Galleries"))
            {
                Directory.CreateDirectory("Galleries");
            }

            if(IsDownloading)
            {
                if (!Directory.Exists("Galleries/" + downloader.name))
                {
                    Directory.CreateDirectory("Galleries/" + downloader.name);
                }
            }
        }

        public static void Hide(string path, bool hide)
        {
            DirectoryInfo directory = new DirectoryInfo(path)
            {
                Attributes = hide ? FileAttributes.Hidden : FileAttributes.Normal
            };
        }

        public static async void Convert(string path)
        {
            string[] files = Directory.GetFiles(path, "*.webm", SearchOption.AllDirectories);
            int total = files.Length;
            for(int i = 0; i < files.Length;i++)
            {
                string source = files[i];
                string output = files[i].Replace(".webm", ".mp4");

                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                ffMpeg.ConvertMedia(source, output, NReco.VideoConverter.Format.mp4);

                Console.WriteLine("Converted " + i + " / " + total);

                GC.Collect();
                await Task.Delay(100);
            }
        }
    }
}
