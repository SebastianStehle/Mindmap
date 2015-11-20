// ==========================================================================
// AttachmentIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using GP.Windows;

namespace Hercules.Model
{
    public sealed class AttachmentIcon : INodeIcon
    {
        private const double MaxSize = 80;
        private const string PropertyKey_Attachment = "Attachment";
        private const string PropertyKey_Name = "Name";
        private readonly string base64Content;
        private readonly string name;

        public string Base64Content
        {
            get { return base64Content; }
        }

        public string Name
        {
            get { return name; }
        }

        public AttachmentIcon(string base64Content, string name)
        {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotNullOrEmpty(base64Content, nameof(base64Content));

            this.base64Content = base64Content;

            this.name = name;
        }

        public AttachmentIcon(Stream stream, string name)
        {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotNull(stream, nameof(stream));

            this.name = name;

            byte[] buffer = stream.ReadToEnd();

            base64Content = Convert.ToBase64String(buffer);
        }

        public static async Task<bool> ValidateAsync(IRandomAccessStream stream)
        {
            bool isValid = false;

            if (stream != null)
            {
                BitmapImage bmp = new BitmapImage();

                try
                {
                    await bmp.SetSourceAsync(stream);

                    if (bmp.PixelWidth < MaxSize && bmp.PixelHeight < MaxSize)
                    {
                        isValid = true;
                    }
                }
                catch
                {
                    isValid = false;
                }
                finally
                {
                    stream.Seek(0);
                }
            }

            return isValid;
        }

        public void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyKey_Attachment, base64Content);
        }

        public Stream ToStream()
        {
            return new MemoryStream(Convert.FromBase64String(base64Content));
        }

        public static INodeIcon TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            INodeIcon result = null;

            if (properties.Contains(PropertyKey_Attachment) && properties.Contains(PropertyKey_Name))
            {
                result = new AttachmentIcon(properties[PropertyKey_Attachment].ToString(), properties[PropertyKey_Name].ToString());
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AttachmentIcon);
        }

        public bool Equals(INodeIcon other)
        {
            return Equals(other as AttachmentIcon);
        }

        public bool Equals(AttachmentIcon other)
        {
            return other != null && other.base64Content == base64Content;
        }

        public override int GetHashCode()
        {
            return base64Content.GetHashCode();
        }
    }
}
