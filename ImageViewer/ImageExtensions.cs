using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageViewer
{
    public static class ImageExtensions
    {
        private const int SamplingDepth = 2;

        /* 
         * Process adapted from http://keyboardimageviewer.codeplex.com/SourceControl/latest#BuildProcessTemplates/UpgradeTemplate.xaml 
         */
        public static Color GetBackgroundColour(this Image image, Color defaultColour)
        {
            var bitmap = image as Bitmap;

            if (bitmap == null)
            {
                return defaultColour;
            }

            if (bitmap.IsAnimated())
            {
                return defaultColour;
            }

            var colourIndex = new ColorIndex(bitmap.Height * SamplingDepth);

            using (var bitmapData = new ImagePixelLock(bitmap))
            {
                CollectColourDataForImageSection(bitmapData.Data, colourIndex, 0, 0, SamplingDepth, bitmap.Height);
                CollectColourDataForImageSection(bitmapData.Data, colourIndex, bitmap.Width - SamplingDepth, 0, SamplingDepth, bitmap.Height);
            }

            var totalPixels = colourIndex.TotalRecords;
            var redComponent = colourIndex.Sum(x => x.Key.R * x.Value) / totalPixels;
            var greenComponent = colourIndex.Sum(x => x.Key.G * x.Value) / totalPixels;
            var blueComponent = colourIndex.Sum(x => x.Key.B * x.Value) / totalPixels;

            var colour = Color.FromArgb(redComponent, greenComponent, blueComponent);
            return colour;
        }

        private static bool IsAnimated(this Image image)
        {
            return image.GetFrameCount(new FrameDimension(image.FrameDimensionsList[0])) > 1;
        }

        private static void CollectColourDataForImageSection(BitmapData bitmapData, ColorIndex colourIndex, int xOffset, int yOffset, int width, int height)
        {
            //var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var stride = bitmapData.Stride;
            var scan0 = bitmapData.Scan0;

            const int red = 2;
            const int green = 1;
            const int blue = 0;

            unsafe
            {
                byte* imageBytePointer = (byte*)(void*)scan0;

                for (int y = yOffset; y < yOffset + height; y++)
                {
                    for (int x = xOffset; x < xOffset + width; x++)
                    {
                        var pixelStart = (y * stride) + x * 4; // ImageLock specifies padding to 32bits per pixel so need the *4
                        var colour = Color.FromArgb(imageBytePointer[pixelStart + red], imageBytePointer[pixelStart + green], imageBytePointer[pixelStart + blue]);
                        colourIndex.Increment(colour);
                    }
                }
            }

            //bitmap.UnlockBits(bitmapData);
        }
    }
}
