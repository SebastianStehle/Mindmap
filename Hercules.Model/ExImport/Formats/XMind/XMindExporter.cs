// ==========================================================================
// xMindExporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;
using GP.Windows;
using Hercules.Model.Rendering;

namespace Hercules.Model.ExImport.Formats.XMind
{
    public sealed class XMindExporter : IExporter
    {
        public string NameKey
        {
            get { return "XMind"; }
        }

        public IEnumerable<FileExtension> Extensions
        {
            get { yield return FileExtension.XMIND; }
        }

        public Task ExportAsync(Document document, IRenderer renderer, Stream stream, PropertiesBag properties = null)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));

            return Task.Run(() =>
            {
                List<KeyValuePair<string, Document>> result = new List<KeyValuePair<string, Document>>();

                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    archive.CreateXmlEntry("META-INF/manifest.xml", WriteManifest);
                    archive.CreateXmlEntry("content.xml", c => ContentWriter.WriteContent(document, c));
                    archive.CreateXmlEntry("styles.xml", c => StylesWriter.WriteContent(document, c, renderer));
                }

                return result;
            });
        }

        private static void WriteManifest(XDocument document)
        {
            document.Add(
                new XElement(Namespaces.Manifest("manifest"),
                    new XElement(Namespaces.Manifest("file-entry"),
                        new XAttribute("full-path", "content.xml"),
                        new XAttribute("media-type", "text/xml")),
                    new XElement(Namespaces.Manifest("file-entry"),
                        new XAttribute("full-path", "styles.xml"),
                        new XAttribute("media-type", "text/xml")),
                    new XElement(Namespaces.Manifest("file-entry"),
                        new XAttribute("full-path", "META-INF"),
                        new XAttribute("media-type", string.Empty)),
                    new XElement(Namespaces.Manifest("file-entry"),
                        new XAttribute("full-path", "META-INF/manifest.xml"),
                        new XAttribute("media-type", "text/xml"))));
        }
    }
}
