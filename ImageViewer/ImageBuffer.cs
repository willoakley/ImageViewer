using System.Collections.Generic;
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

        public void Load(string directoryPath)
        {
            var paths = Loader.ListImagePaths(directoryPath).OrderBy(x => x.LastWriteTime);
            Images = new List<ImageInfo>(paths.Select(x => LoadingImage));
        }
    }
}