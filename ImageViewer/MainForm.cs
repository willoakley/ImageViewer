using System;
using System.Drawing;
using System.IO;
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

            try
            {
                var fileInfo = new FileInfo(arguments[0]);
                SetImage(Image.FromFile(fileInfo.FullName), fileInfo.Name);
            }
            catch (Exception)
            {
                return;
            }
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

        private void SetImage(Image image, string name)
        {
            ImageHolder.Image = image;
            ImageNameBox.Text = name;
            ImageHolder.SizeMode = PictureBoxSizeMode.Zoom; // scale image to fit box

            var zoom = ((image.Width > image.Height) ? ((float)image.Width / Width) : ((float)image.Height / Height)) * 100;

            StatusMessagesBox.Text = String.Format("{0:0}%", zoom);
            
            ImageHolder.Refresh();
        }

        private static void Quit()
        {
            Application.Exit();
        }
    }
}
