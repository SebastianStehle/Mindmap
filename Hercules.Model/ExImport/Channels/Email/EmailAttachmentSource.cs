// ==========================================================================
// EmailAttachmentSource.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using GP.Windows;
using Hercules.Model.Rendering;

namespace Hercules.Model.ExImport.Channels.Email
{
    internal sealed class EmailAttachmentSource : IRandomAccessStreamReference
    {
        private readonly Document document;
        private readonly IExporter exporter;
        private readonly IRenderer renderer;

        public EmailAttachmentSource(Document document, IExporter exporter, IRenderer renderer)
        {
            this.document = document;
            this.exporter = exporter;
            this.renderer = renderer;
        }

        public IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync()
        {
            return ExportAsync().AsAsyncOperation<IRandomAccessStreamWithContentType>();
        }

        private async Task<IRandomAccessStreamWithContentType> ExportAsync()
        {
            FileExtension extension = exporter.Extensions.FirstOrDefault();
            MemoryStream memoryStream = new MemoryStream();

            await exporter.ExportAsync(document, renderer, memoryStream);

            memoryStream.Position = 0;

            return new StreamWrapper(memoryStream, extension.MimeType);
        }
    }
}
