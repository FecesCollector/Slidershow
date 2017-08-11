using System.IO;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Slidershow
{
    public class ImageForm : Form
    {
        bool fullscreen;
        public ImageWindow window;

        public bool Fullscreen
        {
            get
            {
                return fullscreen;
            }
            set
            {
                fullscreen = value;
                window.Dispatcher.Invoke(SetFullscreen);
            }
        }
        
        public void SetFullscreen()
        {
            if (fullscreen)
            {
                BackColor = Color.Black;
                TopMost = true;
            }
            else
            {
                BackColor = Color.Lime;
                TopMost = false;
            }

            window.Draw();
        }

        public ImageForm()
        {
            Top = 0;
            Left = 0;
            Width = Program.Width;
            Height = Program.Height;
            
            AllowTransparency = true;
            BackgroundImageLayout = ImageLayout.Zoom;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            TransparencyKey = Color.Lime;
            FormBorderStyle = FormBorderStyle.None;

            window = new ImageWindow();
            System.Windows.Forms.Integration.ElementHost host = new System.Windows.Forms.Integration.ElementHost()
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = Size,
                Child = window
            };
            
            Controls.Add(host);

            Fullscreen = false;
        }


        public void Press(Keys key)
        {
            var current = Program.current;
            bool refesh = false;
            if (key == Keys.Left || key == Keys.A)
            {
                current.Previous();
                refesh = true;
            }
            if (key == Keys.Right || key == Keys.D)
            {
                current.Next();
                refesh = true;
            }
            if (key == Keys.R || key == Keys.F5)
            {
                current.Load();
                refesh = true;
            }
            if (key == Keys.F)
            {
                Fullscreen = !Fullscreen;
            }
            if (key == Keys.G)
            {
                current.Animations = !current.Animations;
                refesh = true;
            }
            if (key == Keys.H)
            {
                current.Images = !current.Images;
                refesh = true;
            }
            if (key == Keys.J)
            {
                current.Videos = !current.Videos;
                refesh = true;
            }
            if (key == Keys.K)
            {
                current.Random = !current.Random;
                refesh = true;
            }

            if (refesh)
            {
                SetImage(current.Current);
            }

            if (key == Keys.Escape)
            {
                window.Dispatcher.Invoke(Close);
            }
        }

        public void SetImage(string path)
        {
            window.SetSource(path);
            GC.Collect();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            window.mediaElement.Close();
            Program.interceptor.Die();
        }
    }
}
