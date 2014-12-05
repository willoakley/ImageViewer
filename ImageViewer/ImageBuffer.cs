using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageViewer
{
    internal class ImageBuffer
    {
        public List<ImageInfo> Images { get; private set; }

        private ImageInfoLoader Loader { get; set; }
        private Task[] loadingTasks;
        private int index;

        private const int BufferLowerOffset = -2;
        private const int BufferUpperOffset = 5;

        public readonly ImageInfo LoadingImage;
        private List<string> paths;

        public ImageBuffer(ImageInfoLoader loader)
        {
            Loader = loader;
            Images = new List<ImageInfo>();
            index = -1;

            LoadingImage = Loader.LoadingImage();
        }

        public int Count()
        {
            return Images.Count;
        }

        public void Load(string initialImagePath)
        {
            // Use later for selecting initial image to buffer from
            var imageName = Path.GetFullPath(initialImagePath);

            paths = Loader.ListImagePaths(imageName).OrderBy(x => x.LastWriteTime).Select(s => s.FullName).ToList();
            index = paths.FindIndex(n => string.Equals(n, imageName, StringComparison.InvariantCultureIgnoreCase));
            Images = new List<ImageInfo>(paths.Select(x => LoadingImage));

            loadingTasks = new Task[Images.Count];

            AddLoadImage(index); // Selected image first

            AddToBuffer();
        }

        public ImageInfo CurrentImage()
        {
            return Images[index];
        }

        public int CurrentIndex()
        {
            return index + 1;
        }

        public void Next()
        {
            RemoveLoadImage(Normalise(index + BufferLowerOffset));
            index = Normalise(index + 1);
            AddLoadImage(Normalise(index + BufferUpperOffset));
        }

        public void Previous()
        {
            RemoveLoadImage(Normalise(index + BufferUpperOffset));
            index = Normalise(index - 1);
            AddLoadImage(Normalise(index + BufferLowerOffset));
        }

        private void AddToBuffer()
        {
            for (var pos = index + 1; pos < index + BufferUpperOffset; pos++)
            {
                AddLoadImage(Normalise(pos));
            }

            for (var pos = index + BufferLowerOffset; pos < index; pos++)
            {
                AddLoadImage(Normalise(pos));
            }
        }

        private void RemoveLoadImage(int pos)
        {
            Images[pos] = LoadingImage;
            loadingTasks[pos] = null;
        }

        private void AddLoadImage(int pos)
        {
            if (loadingTasks[pos] != null)
            {
                return;
            }

            loadingTasks[pos] = Task.Factory.StartNew(() => LoadImage(pos));
        }

        private void LoadImage(int pos)
        {
            Images[pos] = Loader.FromFile(paths[pos]);

            // todo Fire event with pos that image has been updated
        }

        private int Normalise(int pos)
        {
            if (pos >= 0 && pos < Images.Count)
            {
                return pos;
            }

            if (pos >= Images.Count)
            {
                return pos % Images.Count;
            }

            var lowerPos = Images.Count + pos; // pos is negative
            return lowerPos < 0 ? 0 : lowerPos;
        }
    }
}