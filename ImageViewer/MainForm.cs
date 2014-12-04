using System;
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
                    return;
                }
                case Keys.Back:
                {
                    Quit();
                    return;
                }
                default:
                {
                    MessageBox.Show(string.Concat("No event for ", keyEventArgs.KeyCode));
                    break;
                }
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
            DisplayImage(imageBuffer.Images[0]);

            StatusMessagesBox.Text += string.Format(" {0} images", imageBuffer.Count());
        }

        private void DisplayImage(ImageInfo imageInfo)
        {
            ImageHolder.Image = imageInfo.Picture;
            ImageNameBox.Text = imageInfo.Name;
            ImageHolder.SizeMode = imageInfo.NeverResize ? PictureBoxSizeMode.CenterImage : PictureBoxSizeMode.Zoom; // scale image to fit box

            var zoom = imageInfo.GetZoomPercentage(ImageHolder.Size);
            StatusMessagesBox.Text = String.Format("{0:0}%", zoom);

            BackColor = imageInfo.EdgeColour;
            StatusMessagesBox.BackColor = imageInfo.EdgeColour;
            ImageNameBox.BackColor = imageInfo.EdgeColour;
            
            ImageHolder.Refresh();
            StatusMessagesBox.Refresh();
            ImageNameBox.Refresh();
        }

        private static void Quit()
        {
            Application.Exit();
        }
    }
}
