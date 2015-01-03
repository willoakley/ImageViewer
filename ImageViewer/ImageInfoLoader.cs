using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;

namespace ImageViewer
{
    public class ImageInfoLoader
    {
        private static readonly string[] ValidFileExtensions = { "*.jpg", "*.jpeg", "*.gif", "*.png", "*.tiff", "*.bmp" };

        public ImageInfo FromFile(FileSystemInfo imageFileSystemInfo)
        {
            var image = Image.FromFile(imageFileSystemInfo.FullName);

            return new ImageInfo(image, imageFileSystemInfo.Name, imageFileSystemInfo.FullName, imageFileSystemInfo.LastWriteTimeUtc);
        }

        public ImageInfo FromFile(string relativePath)
        {
            var fileInfo = new FileInfo(relativePath);
            var image = Image.FromFile(relativePath);

            return new ImageInfo(image, fileInfo.Name, relativePath, fileInfo.LastWriteTimeUtc);
        }

        public ImageInfo FromImage(Image image, string name)
        {
            return new ImageInfo(image, name, string.Empty, DateTime.UtcNow);
        }

        public ImageInfo LoadingImage()
        {
            const bool neverResize = true;

            var filename = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "spinner.gif");
            return new ImageInfo(Image.FromFile(filename), "loading", string.Empty, DateTime.UtcNow, neverResize);
        }

        public List<FileSystemInfo> ListImagePaths(string relativePath)
        {
            var directory = new FileInfo(relativePath).Directory;

            if (directory == null)
            {
                return new List<FileSystemInfo>();
            }

            var allImages = new List<FileSystemInfo>();

            // Not worth making each of these its own thread as it actually takes longer
            foreach (var extension in ValidFileExtensions)
            {
                allImages.AddRange(directory.GetFiles(extension, SearchOption.TopDirectoryOnly));
            }

            return allImages;
        }

        public void RecycleImage(string relativePath)
        {
            if (!FileSystem.FileExists(relativePath))
            {
                return;
            }

            FileSystem.DeleteFile(relativePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
        }
    }
}
