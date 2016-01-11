// ==========================================================================
// XMindImporter.cs
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
using GP.Utils;

namespace Hercules.Model.ExImport.Formats.XMind
{
    public sealed class XMindImporter : IImporter
    {
        public string NameKey
        {
            get { return "XMind"; }
        }

        public IEnumerable<FileExtension> Extensions
        {
            get { yield return FileExtension.XMIND; }
        }

        public Task<List<ImportResult>> ImportAsync(Stream stream, string name, PropertiesBag properties = null)
        {
            Guard.NotNull(stream, nameof(stream));

            return Task.Run(() =>
            {
                List<ImportResult> result = new List<ImportResult>();

                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    Dictionary<string, XMindStyle> stylesById = new Dictionary<string, XMindStyle>();

                    ImportStyles(archive, stylesById);

                    ImportContent(archive, stylesById, result);
                }

                return result;
            });
        }

        private static void ImportStyles(ZipArchive archive, IDictionary<string, XMindStyle> stylesById)
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

        private static void ImportContent(ZipArchive archive, IReadOnlyDictionary<string, XMindStyle> stylesById, List<ImportResult> result)
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
