﻿using System;
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
        private readonly Timer imageStillUnloadedTimer;

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
            imageStillUnloadedTimer = new Timer { Interval = 200 };
            imageStillUnloadedTimer.Tick += ImageStillUnloadedTimerTick;
            
            InitializeComponent();

            taskImageBufferLoad = Task.Factory.StartNew(() => imageBuffer.Load(arguments[0]));
            imageStillUnloadedTimer.Start();

            ImageHolder.Image = imageBuffer.LoadingImage.Picture;
        }

        private void ImageStillUnloadedTimerTick(object sender, EventArgs eventArgs)
        {
            imageStillUnloadedTimer.Stop();
            if (ImageNameBox.Text != Squish(imageBuffer.LoadingImage.Name))
            {
                return;
            }

            if (imageBuffer.CurrentImage() == imageBuffer.LoadingImage)
            {
                imageBuffer.ReloadCurrentImageFromDisk();
            }

            DisplayImage(imageBuffer.CurrentImage());
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

                    GoToPreviousImage();
                    return;
                }
                case Keys.Right:
                {
                    if (inActualSizeMode)
                    {
                        MoveImageHolder(Direction.Right);
                        return;
                    }

                    GoToNextImage();
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
                case Keys.Delete:
                {
                    var imageToDelete = imageBuffer.CurrentImage();
                    GoToPreviousImage();
                    imageBuffer.RecycleCurrentImage(imageToDelete);

                    UpdateStatusMessageBoxText(imageBuffer.CurrentImage());
                    StatusMessagesBox.Refresh();
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

        private void GoToNextImage()
        {
            imageBuffer.Next();
            inActualSizeMode = false;
            ImageHolder.Location = new Point(0, 0);
            DisplayImage(imageBuffer.CurrentImage());
            imageStillUnloadedTimer.Start();
        }

        private void GoToPreviousImage()
        {
            imageBuffer.Previous();
            inActualSizeMode = false;
            ImageHolder.Location = new Point(0, 0);
            DisplayImage(imageBuffer.CurrentImage());
            imageStillUnloadedTimer.Start();
        }

        private void MoveImageHolder(Direction direction)
        {
            var horisontalShift = imageBuffer.CurrentImage().Picture.Width / 25;
            var verticalShift = imageBuffer.CurrentImage().Picture.Height / 25;

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

            UpdateImageHolderSizeMode(imageInfo);
            UpdateStatusMessageBoxText(imageInfo);

            ImageHolder.Refresh();
            StatusMessagesBox.Refresh();
        }

        private void UpdateImageHolderSizeMode(ImageInfo imageInfo)
        {
            var inFullyZoomedMode = inActualSizeMode || imageInfo.NeverResize;
            var pictureSize = imageInfo.Picture.Size;
            var screenCentre = new Size(Width / 2, Height / 2);

            ImageHolder.SizeMode = inFullyZoomedMode ? PictureBoxSizeMode.CenterImage : PictureBoxSizeMode.Zoom;
            ImageHolder.Location = inFullyZoomedMode ? new Point(screenCentre.Width - (pictureSize.Width / 2), screenCentre.Height - (pictureSize.Height / 2)) : new Point(0, 0);
            ImageHolder.Size = inFullyZoomedMode ? imageInfo.Picture.Size : Size;
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
            var zoom = inActualSizeMode || imageInfo.NeverResize ? 100 : imageInfo.GetZoomPercentage(ImageHolder.Size);
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
