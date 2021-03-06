﻿// ==========================================================================
// AttachmentIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GP.Utils;

// ReSharper disable InvertIf

namespace Hercules.Model
{
    public sealed class AttachmentIcon : INodeIcon
    {
        public static Func<string, MemoryStream, Task<AttachmentIcon>> Loader;

        public const double MaxSize = 200;

        private const string NameFallback = "Attachment.png";
        private const string PropertyAttachment = "Attachment";
        private const string PropertyName = "Name";
        private readonly string name;
        private readonly byte[] pixelData;
        private readonly int pixelWidth;
        private readonly int pixelHeight;

        public byte[] PixelData
        {
            get { return pixelData; }
        }

        public int PixelWidth
        {
            get { return pixelWidth; }
        }

        public int PixelHeight
        {
            get { return pixelHeight; }
        }

        public string Name
        {
            get { return name; }
        }

        public AttachmentIcon(string name, byte[] pixelData, int pixelWidth, int pixelHeight)
        {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotNull(pixelData, nameof(pixelData));
            Guard.Between(pixelWidth, 0, MaxSize, nameof(pixelWidth));
            Guard.Between(pixelHeight, 0, MaxSize, nameof(pixelWidth));

            this.name = name;

            this.pixelData = pixelData;
            this.pixelWidth = pixelWidth;
            this.pixelHeight = pixelHeight;
        }

        public static Task<AttachmentIcon> TryCreateAsync(string name, string base64Content)
        {
            return TryCreateAsync(name, new MemoryStream(Convert.FromBase64String(base64Content)));
        }

        public static Task<AttachmentIcon> TryCreateAsync(string name, MemoryStream stream)
        {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotNull(stream, nameof(stream));

            return Loader?.Invoke(name, stream);
        }

        public void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyAttachment, Convert.ToBase64String(pixelData));
        }

        public Task<Stream> OpenAsStreamAsync()
        {
            Stream memoryStream = new MemoryStream(pixelData);

            return Task.FromResult(memoryStream);
        }

        public Stream ToStream()
        {
            return new MemoryStream(pixelData);
        }

        public static INodeIcon TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            string attachment;

            if (properties.TryParseString(PropertyAttachment, out attachment) && !string.IsNullOrWhiteSpace(attachment) && attachment.IsBase64Encoded())
            {
                string name;

                if (!properties.TryParseString(PropertyName, out name) || string.IsNullOrWhiteSpace(name))
                {
                    name = NameFallback;
                }

                return TryCreateAsync(name, attachment).Result;
            }

            return null;
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
            return other != null && other.pixelWidth == pixelWidth && other.pixelHeight == pixelHeight && pixelData.SequenceEqual(other.pixelData);
        }

        public override int GetHashCode()
        {
            return pixelData.GetHashCode();
        }
    }
}
