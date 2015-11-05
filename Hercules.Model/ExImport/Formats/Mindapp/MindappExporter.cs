// ==========================================================================
// MindappExporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GP.Windows;
using Hercules.Model.Rendering;
using Hercules.Model.Storing.Json;

namespace Hercules.Model.ExImport.Formats.Mindapp
{
    public sealed class MindappExporter : IExporter
    {
        public string NameKey
        {
            get
            {
                return "Mindapp";
            }
        }

        public IEnumerable<FileExtension> Extensions
        {
            get
            {
                yield return JsonDocumentSerializer.FileExtension;
            }
        }

        public Task ExportAsync(Document document, IRenderer renderer, Stream stream, PropertiesBag properties = null)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(stream, nameof(stream));

            return Task.Run(() =>
            {
                JsonDocumentSerializer.SerializeToStream(stream, document);
            });
        }
    }
}
