// ==========================================================================
// xMindImporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using GP.Windows;

namespace Hercules.Model.ExImport.Formats.xMind
{
    public sealed class xMindImporter : IImporter
    {
        private static readonly Regex ColorRegex = new Regex("^#[0-9A-F]{6}$", RegexOptions.Compiled);

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

                using (ZipArchive archive = new ZipArchive(stream))
                {
                    Dictionary<string, xMindStyle> stylesById = new Dictionary<string, xMindStyle>();

                    ParseStyles(archive, stylesById);

                    ParseStructure(archive, stylesById, result);
                }

                return result;
            });
        }

        private static void ParseStyles(ZipArchive archive, IDictionary<string, xMindStyle> stylesById)
        {
            ZipArchiveEntry mapStylesEntry = archive.GetEntry("styles.xml");

            if (mapStylesEntry != null)
            {
                using (Stream stream = mapStylesEntry.Open())
                {
                    XDocument mapStyles = XDocument.Load(stream);

                    CheckVersion(mapStyles);
                    
                    XElement nodeStyles = mapStyles.Root.Element(StylesXName("styles"));

                    if (nodeStyles != null)
                    {
                        foreach (XElement style in nodeStyles.Elements(StylesXName("style")))
                        {
                            string id = style.Attribute("id")?.Value;

                            if (string.IsNullOrWhiteSpace(id))
                            {
                                continue;
                            }

                            string type = style.Attribute("type")?.Value;

                            if (string.IsNullOrWhiteSpace(type) || !string.Equals(type, "topic", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            XElement properties = style.Element(StylesXName("topic-properties"));

                            if (properties == null)
                            {
                                continue;
                            }

                            string fillString = properties.Attribute(SVGXName("fill"))?.Value;

                            if (string.IsNullOrWhiteSpace(fillString) || !ColorRegex.IsMatch(fillString))
                            {
                                continue;
                            }

                            int color;

                            if (int.TryParse(fillString.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color))
                            {
                                stylesById[id] = new xMindStyle { Color = color };
                            }
                        }
                    }
                }
            }
        }

        private static void ParseStructure(ZipArchive archive, IReadOnlyDictionary<string, xMindStyle> stylesById, ICollection<KeyValuePair<string, Document>> result)
        {
            ZipArchiveEntry contentEntry = archive.GetEntry("content.xml");

            if (contentEntry == null)
            {
                throw new IOException("Content.xml not found.");
            }

            using (Stream stream = contentEntry.Open())
            {
                XDocument content = XDocument.Load(stream);

                CheckVersion(content);

                IEnumerable<XElement> sheets = content.Root.Elements(ContentXName("sheet"));

                foreach (XElement sheet in sheets)
                {
                    string title = sheet.Element(ContentXName("title"))?.Value;

                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        Document document = new Document(Guid.NewGuid());

                        ParseSheet(sheet, document, stylesById);

                        result.Add(new KeyValuePair<string, Document>(title, document));
                    }
                }
            }
        }

        private static void CheckVersion(XDocument content)
        {
            if (content.Root.Attribute("version")?.Value != "2.0")
            {
                throw new IOException("Mindmap has an unsupported version.");
            }
        }

        private static void ParseSheet(XContainer sheet, Document document, IReadOnlyDictionary<string, xMindStyle> stylesById)
        {
            XElement root = sheet.Element(ContentXName("topic"));

            if (root != null)
            {
                HandleNode(root, document.Root, stylesById);
            }
        }

        private static void HandleNode(XElement topic, NodeBase node, IReadOnlyDictionary<string, xMindStyle> stylesById)
        {
            ParseTitle(topic, node);
            ParseStyle(topic, node, stylesById);
            ParseChild(topic, node, stylesById);
        }

        private static void ParseStyle(XElement topic, NodeBase node, IReadOnlyDictionary<string, xMindStyle> stylesById)
        {
            string styleId = topic.Attribute("style-id")?.Value;

            xMindStyle style;

            if (!string.IsNullOrWhiteSpace(styleId) && stylesById.TryGetValue(styleId, out style))
            {
                if (style.Color >= 0)
                {
                    node.ChangeColorTransactional(new CustomColor(style.Color));
                }
            }
        }

        private static void ParseChild(XContainer topic, NodeBase node, IReadOnlyDictionary<string, xMindStyle> stylesById)
        {
            XElement topics = topic.Element(ContentXName("children"))?.Element(ContentXName("topics"));

            if (topics != null)
            {
                foreach (XElement subtopic in topics.Elements(ContentXName("topic")))
                {
                    NodeBase child = node.AddChildTransactional();

                    HandleNode(subtopic, child, stylesById);
                }
            }
        }

        private static void ParseTitle(XContainer topic, NodeBase node)
        {
            string title = topic.Element(ContentXName("title"))?.Value;

            if (!string.IsNullOrWhiteSpace(title))
            {
                node.ChangeTextTransactional(title);
            }
        }

        private static string SVGXName(string name)
        {
            return "{http://www.w3.org/2000/svg}" + name;
        }

        private static string ContentXName(string name)
        {
            return "{urn:xmind:xmap:xmlns:content:2.0}" + name;
        }

        private static string StylesXName(string name)
        {
            return "{urn:xmind:xmap:xmlns:style:2.0}" + name;
        }
    }
}
