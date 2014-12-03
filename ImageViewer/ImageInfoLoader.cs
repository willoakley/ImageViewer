using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImageViewer
{
    public class ImageInfoLoader
    {
        private static readonly string[] ValidFileExtensions = { "*.jpg", "*.jpeg", "*.gif", "*.png", "*.tiff", "*.bmp" };

        public ImageInfo FromFile(FileSystemInfo imageFileSystemInfo)
        {
            var image = Image.FromFile(imageFileSystemInfo.FullName);

            return new ImageInfo(image, imageFileSystemInfo.Name, imageFileSystemInfo.FullName);
        }

        public ImageInfo FromFile(string relativePath)
        {
            var fileInfo = new FileInfo(relativePath);
            var image = Image.FromFile(relativePath);

            return new ImageInfo(image, fileInfo.Name, relativePath);
        }

        public ImageInfo FromImage(Image image, string name)
        {
            return new ImageInfo(image, name, string.Empty);
        }

        public List<FileSystemInfo> ListImagePaths(string relativePath)
        {
            var directory = new FileInfo(relativePath).Directory;

            if (directory == null)
            {
                return new List<FileSystemInfo>();
            }

            var allImages = new List<FileSystemInfo>();

            foreach (var type in ValidFileExtensions)
            {
                allImages.AddRange(directory.GetFiles(type, SearchOption.TopDirectoryOnly));
            }

            return allImages;
        }
    }
}
