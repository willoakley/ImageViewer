using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer
{
    public partial class MainForm : Form
    {
        private readonly Task<ImageInfo> loadInitialImage;
        private ImageInfo initialImage;

        public MainForm(string[] arguments)
        {
            if (arguments == null || string.IsNullOrEmpty(arguments[0]))
            {
                return;
            }

            loadInitialImage = Task.Factory.StartNew(() => new ImageInfoLoader().FromFile(arguments[0]));

            InitializeComponent();
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

            if (loadInitialImage.IsFaulted)
            {
                Quit();
            }

            initialImage = loadInitialImage.Result;
            DisplayImage(initialImage);

            var allImages = new ImageInfoLoader().ListImagePaths(initialImage.RelativePath);
            StatusMessagesBox.Text += string.Format(" {0} images", allImages.Count);
        }

        private void DisplayImage(ImageInfo imageInfo)
        {
            ImageHolder.Image = imageInfo.Picture;
            ImageNameBox.Text = imageInfo.Name;
            ImageHolder.SizeMode = PictureBoxSizeMode.Zoom; // scale image to fit box

            var zoom = imageInfo.GetZoomPercentage(ImageHolder.Size);
            StatusMessagesBox.Text = String.Format("{0:0}%", zoom);

            BackColor = imageInfo.EdgeColour;
            
            ImageHolder.Refresh();
        }

        private static void Quit()
        {
            Application.Exit();
        }
    }
}
