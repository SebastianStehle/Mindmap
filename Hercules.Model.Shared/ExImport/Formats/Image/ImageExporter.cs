// ==========================================================================
// ImageExporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using GP.Utils;
using Hercules.Model.Rendering;

namespace Hercules.Model.ExImport.Formats.Image
{
    public sealed class ImageExporter : IExporter
    {
        public string NameKey
        {
            get { return "Image"; }
        }

        public IEnumerable<FileExtension> Extensions
        {
            get { yield return FileExtension.PNG; }
        }

        public Task ExportAsync(Document document, IRenderer renderer, Stream stream, PropertiesBag properties = null)
        {
            int dpi =
                properties != null &&
                properties.Contains("DPI") ?
                properties["DPI"].ToInt32(CultureInfo.InvariantCulture) :
                300;

            return renderer.RenderScreenshotAsync(stream, new Vector3(1, 1, 1), dpi);
        }
    }
}
