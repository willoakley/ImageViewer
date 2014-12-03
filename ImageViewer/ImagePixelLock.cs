using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageViewer
{
    internal class ImagePixelLock : IDisposable
    {
        public BitmapData Data { get; private set; }

        private readonly Bitmap image;

        public ImagePixelLock(Bitmap image)
        {
            this.image = image;

            // Use read-write lock to make system wait for until the interface is finished using the image
            Data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        }

        public void Dispose()
        {
            if (Data != null && image != null)
            {
                image.UnlockBits(Data);
            }
        }
    }
}
