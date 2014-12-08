using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer
{
    public partial class MainForm : Form
    {
        private readonly Task taskImageBufferLoad;
        private readonly ImageBuffer imageBuffer;
        private bool inActualSizeMode;

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public MainForm(string[] arguments)
        {
            if (arguments == null || arguments.Length == 0 || string.IsNullOrEmpty(arguments[0]))
            {
                return;
            }

            imageBuffer = new ImageBuffer(new ImageInfoLoader());
            imageBuffer.ImageLoaded += ImageBuffer_ImageLoaded;
            
            InitializeComponent();

            taskImageBufferLoad = Task.Factory.StartNew(() => imageBuffer.Load(arguments[0]));

            ImageHolder.Image = imageBuffer.LoadingImage.Picture;
        }

        private void ImageBuffer_ImageLoaded(object sender, int loadedImageIndex)
        {
            if (imageBuffer.CurrentIndex() != loadedImageIndex)
            {
                return;
            }

            Invoke(new Action(() => { System.Threading.Thread.Sleep(50); DisplayImage(imageBuffer.CurrentImage()); }));
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
                case Keys.Left:
                {
                    if (inActualSizeMode)
                    {
                        MoveImageHolder(Direction.Left);
                        return;
                    }

                    imageBuffer.Previous();
                    inActualSizeMode = false;
                    ImageHolder.Location = new Point(0, 0);
                    DisplayImage(imageBuffer.CurrentImage());
                    return;
                }
                case Keys.Right:
                {
                    if (inActualSizeMode)
                    {
                        MoveImageHolder(Direction.Right);
                        return;
                    }

                    imageBuffer.Next();
                    inActualSizeMode = false;
                    ImageHolder.Location = new Point(0, 0);
                    DisplayImage(imageBuffer.CurrentImage());
                    return;
                }
                case Keys.Up:
                {
                    if (inActualSizeMode)
                    {
                        MoveImageHolder(Direction.Up);
                    }

                    return;
                }
                case Keys.Down:
                {
                    if (inActualSizeMode)
                    {
                        MoveImageHolder(Direction.Down);
                    }

                    return;
                }
                case Keys.ShiftKey:
                {
                    inActualSizeMode = !inActualSizeMode;
                    ImageHolder.Location = new Point(0, 0);
                    UpdateZoomDisplay();
                    return;
                }
                case Keys.F5:
                {
                    if (imageBuffer.CurrentImage() == imageBuffer.LoadingImage)
                    {
                        imageBuffer.ReloadCurrentImageFromDisk();
                    }

                    DisplayImage(imageBuffer.CurrentImage());
                    return;
                }
#if DEBUG
                default:
                {
                    MessageBox.Show(string.Concat("No event for ", keyEventArgs.KeyCode));
                    return;
                }
#endif
            }
        }

        private void MoveImageHolder(Direction direction)
        {
            var horisontalShift = imageBuffer.CurrentImage().Picture.Width / 20;
            var verticalShift = imageBuffer.CurrentImage().Picture.Height / 20;

            switch (direction)
            {
                case Direction.Up:
                {
                    ImageHolder.Location = new Point(ImageHolder.Location.X, ImageHolder.Location.Y - verticalShift);
                    break;
                }
                case Direction.Down:
                {
                    ImageHolder.Location = new Point(ImageHolder.Location.X, ImageHolder.Location.Y + verticalShift);
                    break;
                }
                case Direction.Left:
                {
                    ImageHolder.Location = new Point(ImageHolder.Location.X - horisontalShift, ImageHolder.Location.Y);
                    break;
                }
                case Direction.Right:
                {
                    ImageHolder.Location = new Point(ImageHolder.Location.X + horisontalShift, ImageHolder.Location.Y);
                    break;
                }
            }

            ImageHolder.Refresh();
        }

        private void UpdateZoomDisplay()
        {
            var imageInfo = imageBuffer.CurrentImage();

            UpdateStatusMessageBoxText(imageInfo);
            UpdateImageHolderSizeMode(imageInfo);

            ImageHolder.Refresh();
            StatusMessagesBox.Refresh();
        }

        private void UpdateImageHolderSizeMode(ImageInfo imageInfo)
        {
            ImageHolder.SizeMode = inActualSizeMode || imageInfo.NeverResize
                ? PictureBoxSizeMode.CenterImage
                : PictureBoxSizeMode.Zoom;
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

            UpdateImageHolderSizeMode(imageInfo);
            UpdateStatusMessageBoxText(imageInfo);

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

        private void UpdateStatusMessageBoxText(ImageInfo imageInfo)
        {
            var zoom = inActualSizeMode ? 100 : imageInfo.GetZoomPercentage(ImageHolder.Size);
            StatusMessagesBox.Text = String.Format("{1} of {2} at {0:0}%", zoom, imageBuffer.CurrentIndex(), imageBuffer.Count());
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
