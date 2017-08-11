using System;
using System.Collections.Generic;
using System.IO;

namespace Slidershow
{
    public class Gallery
    {
        public string name;
        
        List<string> files = new List<string>();
        bool animations = false;
        bool images = false;
        bool randomize = false;
        bool videos = false;
        int index = 0;
        public string path = "";
        Random random;

        public string Current
        {
            get
            {
                if (files.Count == 0) return "";
                if (files.Count <= index) return "";

                return files[index];
            }
        }

        public int Total
        {
            get
            {
                return files.Count;
            }
        }

        public bool Videos
        {
            get
            {
                return videos;
            }
            set
            {
                videos = value;
                Load();
                Program.imageForm.window.Draw();
            }
        }

        public bool Random
        {
            get
            {
                return randomize;
            }
            set
            {
                string lastPath = Current;
                randomize = value;
                random = new Random(DateTime.Now.Millisecond);
                Load();

                for (int i = 0; i < files.Count; i++)
                {
                    if (lastPath == files[i])
                    {
                        index = i;
                        break;
                    }
                }

                Program.imageForm.window.Draw();
            }
        }

        public bool Animations
        {
            get
            {
                return animations;
            }
            set
            {
                animations = value;
                Load();
                Program.imageForm.window.Draw();
            }
        }

        public bool Images
        {
            get
            {
                return images;
            }
            set
            {
                images = value;
                Load();
                Program.imageForm.window.Draw();
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                Load();
                Program.imageForm.window.Draw();
            }
        }

        public Gallery(string path)
        {
            this.path = path;
            name = Path.GetFileNameWithoutExtension(path);

            animations = true;
            images = true;

            Load();
        }

        public static int ValidFiles(string[] files)
        {
            int valids = 0;
            for (int i = 0; i < files.Length; i++)
            {
                string ext = Path.GetExtension(files[i]);

                if ((ext == ".gif") || (ext == ".mp4") || (ext == ".png") || (ext == ".jpg"))
                {
                    long length = new FileInfo(files[i]).Length;
                    if (length != 0)
                    {
                        valids++;
                    }
                }
            }

            return valids;
        }

        public void Load()
        {
            this.files.Clear();

            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                string ext = Path.GetExtension(files[i]);

                if ((ext == ".gif" && animations) || (ext == ".mp4" && videos) || (ext == ".png" && images) || (ext == ".jpg" && images))
                {
                    long length = new FileInfo(files[i]).Length;
                    if(length != 0)
                    {
                        this.files.Add(files[i]);
                    }
                }
            }

            if (randomize)
            {
                int n = this.files.Count;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    string value = this.files[k];
                    this.files[k] = this.files[n];
                    this.files[n] = value;
                }
            }
        }

        public void Next()
        {
            index++;
            if (index >= files.Count)
            {
                index = 0;
            }
            Program.imageForm.window.Draw();
        }

        public void Previous()
        {
            index--;
            if (index < 0)
            {
                if(files.Count > 0)
                {
                    index = files.Count - 1;
                }
                else
                {
                    index = 0;
                }
            }
            Program.imageForm.window.Draw();
        }
    }
}
