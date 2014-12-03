using System;
using System.Windows.Forms;

namespace ImageViewer
{
    public partial class MainForm : Form
    {

        public MainForm(string[] arguments)
        {
            if (arguments == null || string.IsNullOrEmpty(arguments[0]))
            {
                return;
            }

            InitializeComponent();

            var imageInfo = ImageInfo.LoadFromFile(arguments[0]);
            DisplayImage(imageInfo);
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
