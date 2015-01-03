using System.Threading.Tasks;

namespace ImageViewer
{
    internal class ImageBufferImageInfo
    {
        public ImageInfo ImageInfo { get; set; }
        public Task Task { get; set; }
    }
}
