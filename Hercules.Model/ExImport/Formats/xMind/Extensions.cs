// ==========================================================================
// Extensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

namespace Hercules.Model.ExImport.Formats.xMind
{
    public static class Extensions
    {
        public static void CheckVersion(this XDocument content, string expected = "1.0")
        {
            if (content.Root.Attribute("version")?.Value != expected)
            {
                throw new IOException("Mindmap has an unsupported version.");
            }
        }

        public static string ElementValue(this XContainer element, XName name)
        {
            return element.Element(name)?.Value;
        }

        public static string AttributeValue(this XElement element, XName name)
        {
            return element.Attribute(name)?.Value;
        }

        public static bool IsAttributeEquals(this XElement element, XName name, string expected)
        {
            return string.Equals(AttributeValue(element, name), expected, StringComparison.OrdinalIgnoreCase);
        }

        public static void CreateXmlEntry(this ZipArchive archive, string path, Action<XDocument> xmlWriter)
        {
            ZipArchiveEntry contentEntry = archive.CreateEntry(path);

            using (Stream stream = contentEntry.Open())
            {
                XDocument content = new XDocument();

                xmlWriter(content);

                using (XmlWriter writer = XmlWriter.Create(stream))
                {
                    content.WriteTo(writer);
                }
            }
        }
    }
}
