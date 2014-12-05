using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer
{
    public partial class MainForm : Form
    {
        private readonly Task taskImageBufferLoad;
        private readonly ImageBuffer imageBuffer;

        public MainForm(string[] arguments)
        {
            if (arguments == null || string.IsNullOrEmpty(arguments[0]))
            {
                return;
            }

            imageBuffer = new ImageBuffer(new ImageInfoLoader());

            taskImageBufferLoad = Task.Factory.StartNew(() => imageBuffer.Load(arguments[0]));

            InitializeComponent();
            ImageHolder.Image = imageBuffer.LoadingImage.Picture;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.KeyCode)
            {
                case Keys.Escape:
                {
                    Quit();
                    break;
                }
                case Keys.Back:
                {
                    Quit();
                    break;
                }
                case Keys.Left:
                {
                    imageBuffer.Previous();
                    DisplayImage(imageBuffer.CurrentImage());
                    break;
                }
                case Keys.Right:
                {
                    imageBuffer.Next();
                    DisplayImage(imageBuffer.CurrentImage());
                    break;
                }
                case Keys.Shift:
                {
                    // cycle zoom mode
                    break;
                }
#if DEBUG
                default:
                {
                    MessageBox.Show(string.Concat("No event for ", keyEventArgs.KeyCode));
                    break;
                }
#endif
            }
        }

        private void MainForm_Load(object sender, EventArgs eventArgs)
        {
            ImageHolder.Size = Size;

            if (taskImageBufferLoad.IsFaulted)
            {
                Quit();
            }

            taskImageBufferLoad.Wait();
            DisplayImage(imageBuffer.CurrentImage());

            StatusMessagesBox.Text += string.Format(" {0} images", imageBuffer.Count());
        }

        private void DisplayImage(ImageInfo imageInfo)
        {
            ImageHolder.Image = imageInfo.Picture;
            ImageNameBox.Text = Squish(imageInfo.Name);
            ImageHolder.SizeMode = imageInfo.NeverResize ? PictureBoxSizeMode.CenterImage : PictureBoxSizeMode.Zoom; // scale image to fit box

            var zoom = imageInfo.GetZoomPercentage(ImageHolder.Size);
            StatusMessagesBox.Text = String.Format("{1} of {2} at {0:0}%", zoom, imageBuffer.CurrentIndex(), imageBuffer.Count());

            BackColor = imageInfo.EdgeColour;
            StatusMessagesBox.BackColor = imageInfo.EdgeColour;
            ImageNameBox.BackColor = imageInfo.EdgeColour;
            var contrast = ContrastingColor(imageInfo.EdgeColour);
            StatusMessagesBox.ForeColor = contrast;
            ImageNameBox.ForeColor = contrast;
            
            ImageHolder.Refresh();
            StatusMessagesBox.Refresh();
            ImageNameBox.Refresh();
        }

        private static void Quit()
        {
            Application.Exit();
        }

        private static Color ContrastingColor(Color color)
        {
            // Counting the perceptive luminance - human eye favors green color. Adapted from http://stackoverflow.com/questions/1855884/determine-font-color-based-on-background-color  
            var a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return (a < 0.5) ? Color.Gray : Color.LightGray;
        }

        private static string Squish(string text)
        {
            const int chunk = 35;
            return text.Length < chunk * 3 ? text : string.Concat(text.Substring(0, chunk), "...", text.Substring(text.Length - chunk));
        }
    }
}
