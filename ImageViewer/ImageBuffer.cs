using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageViewer
{
    internal class ImageBuffer
    {
        public List<ImageInfo> Images { get; private set; }

        private ImageInfoLoader Loader { get; set; }
        
        public readonly ImageInfo LoadingImage;

        public ImageBuffer(ImageInfoLoader loader)
        {
            Loader = loader;
            Images = new List<ImageInfo>();

            LoadingImage = Loader.LoadingImage();
        }

        public int Count()
        {
            return Images.Count;
        }

        public void Load(string initialImagePath)
        {
            // Use later for selecting initial image to buffer from
            var imageName = Path.GetFileName(initialImagePath);

            var paths = Loader.ListImagePaths(initialImagePath).OrderBy(x => x.LastWriteTime);
            Images = new List<ImageInfo>(paths.Select(x => LoadingImage));
        }
    }
}