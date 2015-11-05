// ==========================================================================
// xMindImporter.cs
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

namespace Hercules.Model.ExImport.Formats.xMind
{
    public sealed class xMindImporter : IImporter
    {
        public string NameKey
        {
            get { return "xMind"; }
        }

        public IEnumerable<FileExtension> Extensions
        {
            get { yield return FileExtension.XMIND; }
        }

        public Task<List<KeyValuePair<string, Document>>> ImportAsync(Stream stream, PropertiesBag properties = null)
        {
            Guard.NotNull(stream, nameof(stream));

            return Task.Run(() =>
            {
                List<KeyValuePair<string, Document>> result = new List<KeyValuePair<string, Document>>();

                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    Dictionary<string, xMindStyle> stylesById = new Dictionary<string, xMindStyle>();

                    ImportStyles(archive, stylesById);

                    ImportContent(archive, stylesById, result);
                }

                return result;
            });
        }

        private static void ImportStyles(ZipArchive archive, IDictionary<string, xMindStyle> stylesById)
        {
            ZipArchiveEntry mapStylesEntry = archive.GetEntry("styles.xml");

            if (mapStylesEntry != null)
            {
                using (Stream stream = mapStylesEntry.Open())
                {
                    XDocument mapStyles = XDocument.Load(stream);

                    mapStyles.CheckVersion("2.0");

                    StylesReader.ReadStyles(mapStyles, stylesById);
                }
            }
        }

        private static void ImportContent(ZipArchive archive, IReadOnlyDictionary<string, xMindStyle> stylesById, List<KeyValuePair<string, Document>> result)
        {
            ZipArchiveEntry contentEntry = archive.GetEntry("content.xml");

            if (contentEntry == null)
            {
                throw new IOException("Content.xml not found.");
            }

            using (Stream stream = contentEntry.Open())
            {
                XDocument content = XDocument.Load(stream);

                content.CheckVersion("2.0");

                result.AddRange(ContentReader.ReadContent(content, stylesById));
            }
        }
    }
}
