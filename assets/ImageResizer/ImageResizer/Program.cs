// ==========================================================================
// Program.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageResizer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            foreach (IconConfig config in IconConfigs.Configs)
            {
                FileInfo file = new FileInfo(config.SourceName);

                Bitmap bitmap = new Bitmap(file.FullName);

                foreach (double scaling in config.Scalings)
                {
                    int w = (int)Math.Ceiling(scaling * config.Size.Width);
                    int h = (int)Math.Ceiling(scaling * config.Size.Height);

                    int scalingDisplay = (int)(scaling * 100);

                    string fileName = config.TargetName.Replace("{scale}", scalingDisplay.ToString());

                    Resize(bitmap, w, h, fileName);

                    Console.WriteLine("Resizing...{0}x{1}", w, h);
                }
            }
        }

        private static void Resize(Bitmap image, int w, int h, string fileName)
        {
            Bitmap newImage = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, w, h);
            }
            
            newImage.Save(fileName, ImageFormat.Png);
        }
    }
}
