using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Slidershow
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : UserControl
    {
        string currentUrl;
        ImageForm form;

        int ScreenWidth
        {
            get
            {
                return form.CurrentScreen.WorkingArea.Width;
            }
        }

        int ScreenHeight
        {
            get
            {
                return form.CurrentScreen.WorkingArea.Height;
            }
        }

        public ImageWindow(ImageForm form)
        {
            InitializeComponent();
            Panel.SetZIndex(this, int.MinValue);
            this.form = form;

            mediaElement.MediaEnded += MediaElement_MediaEnded;
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MaxHeight = ScreenHeight;
            mediaElement.MaxWidth = ScreenWidth;
        }

        Action EmptyDelegate = delegate () { };
        public void Draw()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            Dispatcher.Invoke(DrawAction);
        }

        void DrawAction()
        {
            InvalidateVisual(); //this runs on another thread
            UpdateLayout();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            string testString = "";
            testString += "G = Animations (" + Program.current.Animations + ")\n";
            testString += "H = Images (" + Program.current.Images + ")\n";
            testString += "J = Videos (" + Program.current.Videos + ")\n";
            testString += "K = Randomize (" + Program.current.Random + ")\n";
            testString += "S = Display (" + Program.imageForm.CurrentDisplay + "/" + Program.imageForm.allScreens.Length + ")\n";
            testString += "\n\n";
            testString += (Program.current.Index + 1) + " / " + Program.current.Total;

            // Create the initial formatted text string.
            FormattedText formattedText = new FormattedText(
                testString,
                System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Courier New"),
                12,
                Brushes.White);

            drawingContext.DrawText(formattedText, new Point(5, 5));
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            mediaElement.Stretch = Stretch.Uniform;
            mediaElement.StretchDirection = StretchDirection.DownOnly;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.UnloadedBehavior = MediaState.Manual;
            mediaElement.Position = TimeSpan.FromMilliseconds(1);
            mediaElement.Play();
        }

        string url;
        public void SetSource(string url = "")
        {
            if(url != "")
            {
                this.url = url;
            }
            Dispatcher.Invoke(SetSourceAction);
        }

        void SetSourceAction()
        {
            currentUrl = url;
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)) return;

            url = System.IO.Path.GetFullPath(url);

            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            Width = ScreenWidth;
            Height = ScreenHeight;

            mediaElement.Width = ScreenWidth;
            mediaElement.Height = ScreenHeight;

            mediaElement.HorizontalAlignment = HorizontalAlignment.Center;
            mediaElement.VerticalAlignment = VerticalAlignment.Center;

            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.UnloadedBehavior = MediaState.Manual;

            mediaElement.Source = new Uri(url);
            mediaElement.Play();

            GC.Collect();
        }
    }
}
