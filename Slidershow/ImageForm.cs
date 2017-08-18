using System.IO;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Slidershow
{
    public class ImageForm : Form
    {
        public ImageWindow window;
        int currentDisplay = 0;
        public Screen[] allScreens;

        public int CurrentDisplay
        {
            get
            {
                return currentDisplay + 1;
            }
        }

        public Screen CurrentScreen
        {
            get
            {
                return allScreens[currentDisplay];
            }
        }
        
        public ImageForm()
        {
            allScreens = Screen.AllScreens;
            StartPosition = FormStartPosition.Manual;
            AllowTransparency = true;
            BackgroundImageLayout = ImageLayout.Zoom;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            TransparencyKey = Color.Lime;
            FormBorderStyle = FormBorderStyle.None;

            window = new ImageWindow(this);
            System.Windows.Forms.Integration.ElementHost host = new System.Windows.Forms.Integration.ElementHost()
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = Size,
                Child = window
            };
            
            Controls.Add(host);

            Screen screen = allScreens[0];
            Left = screen.Bounds.Width;
            Top = screen.Bounds.Height;
            Location = screen.Bounds.Location;
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;

            BackColor = Color.Black;
            TopMost = true;
        }

        public void CycleDisplays()
        {
            window.Dispatcher.Invoke(SetCycleDisplay);
        }

        void SetCycleDisplay()
        {
            currentDisplay++;
            if (currentDisplay >= allScreens.Length)
            {
                currentDisplay = 0;
            }

            Screen screen = allScreens[currentDisplay];

            SuspendLayout();

            Left = screen.Bounds.Width;
            Top = screen.Bounds.Height;
            Location = screen.Bounds.Location;
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;

            ResumeLayout();

            window.SetSource();
            window.Draw();
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
            if (key == Keys.S)
            {
                CycleDisplays();
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
