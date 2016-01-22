// ==========================================================================
// ImageSerializer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Hercules.Model
{
    public static class ImageSerializer
    {
        private static readonly Guid[] DecoderIds =
        {
            BitmapDecoder.PngDecoderId,
            BitmapDecoder.JpegDecoderId,
            BitmapDecoder.JpegXRDecoderId
        };

        public static void Setup()
        {
            AttachmentIcon.Loader = TryCreateAsync;

            KeyIcon.StreamProvider = OpenKeyStreamAsync;
        }

        public static async Task<AttachmentIcon> TryCreateAsync(string name, MemoryStream stream)
        {
            AttachmentIcon result = null;

            foreach (Guid decoderId in DecoderIds)
            {
                stream.Position = 0;

                try
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(decoderId, stream.AsRandomAccessStream()).AsTask().ConfigureAwait(false);

                    if (!(decoder.PixelWidth <= AttachmentIcon.MaxSize) || !(decoder.PixelHeight <= AttachmentIcon.MaxSize))
                    {
                        continue;
                    }

                    result = new AttachmentIcon(name, stream.ToArray(), (int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    break;
                }
                catch
                {
                    result = null;
                }
            }

            return result;
        }

        public static async Task<Stream> OpenKeyStreamAsync(string key)
        {
            string uri = $"ms-appx://{key}";

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            return (await file.OpenReadAsync()).AsStreamForRead();
        }
    }
}
