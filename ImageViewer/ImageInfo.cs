﻿using System.Drawing;

namespace ImageViewer
{
    public class ImageInfo
    {
        public ImageInfo(Image image, string name, string relativePath)
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

            var scalledDecimal = (IsWider ? ((float)sizeLimit.Width / Picture.Width) : ((float)sizeLimit.Height / Picture.Height));
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