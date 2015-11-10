// ==========================================================================
// AttachmentIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using GP.Windows;

namespace Hercules.Model
{
    public sealed class AttachmentIcon : INodeIcon
    {
        private const string PropertyKey = "Attachment";
        private readonly string base64Content;

        public string Base64Content
        {
            get { return base64Content; }
        }

        public AttachmentIcon(string base64Content)
        {
            Guard.NotNullOrEmpty(base64Content, nameof(base64Content));

            this.base64Content = base64Content;
        }

        public AttachmentIcon(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            byte[] buffer = stream.ReadToEnd();

            base64Content = Convert.ToBase64String(buffer);
        }

        public void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyKey, base64Content);
        }

        public static INodeIcon TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            return properties.Contains(PropertyKey) ? new AttachmentIcon(properties[PropertyKey].ToString()) : null;
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
