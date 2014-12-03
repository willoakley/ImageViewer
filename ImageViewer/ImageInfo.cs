using System.Drawing;
using System.IO;

namespace ImageViewer
{
    public class ImageInfo
    {
        public static ImageInfo LoadFromFile(string relativePath)
        {
            var fileInfo = new FileInfo(relativePath);
            var image = Image.FromFile(relativePath);

            return new ImageInfo(image, fileInfo.Name, string.Empty);
        }

        public static ImageInfo FromImage(Image image, string name)
        {
            return new ImageInfo(image, name, string.Empty);
        }


        private ImageInfo(Image image, string name, string relativePath)
        {
            Picture = image;
            Name = string.IsNullOrWhiteSpace(name) ? "noname" : name.Trim();
            RelativePath = relativePath;

            EdgeColour = GetEdgeColour();
        }


        public Image Picture { get; private set; }
        public string Name { get; private set; }
        public string RelativePath { get; private set; }
        public Color EdgeColour { get; private set; }


        public int GetZoomPercentage(Size sizeLimit)
        {
            if (Picture == null)
            {
                return 0;
            }

            var scalledDecimal = (IsWider ? ((float)Picture.Width / sizeLimit.Width) : ((float)Picture.Height / sizeLimit.Height));
            return (int)(100 * scalledDecimal);
        }

        public bool IsWider {
            get { return Picture != null && Picture.Width > Picture.Height; }
        }

        public bool IsTaller
        {
            get { return Picture != null && Picture.Height >= Picture.Width; }
        }


        private static Color GetEdgeColour()
        {
            return Color.LightGray;
        }
    }
}
