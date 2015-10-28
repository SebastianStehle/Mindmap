// ==========================================================================
// HtmlOutlineExporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using GP.Windows;
using GP.Windows.UI;
using Hercules.Model.Rendering;

namespace Hercules.Model.Export.Html
{
    public sealed class HtmlOutlineExporter : IOutlineGenerator, IExporter
    {
        private const string ListStyle = "padding-left:18px;";
        private const string ListItemStyle = "padding-top:4px;padding-bottom:4px;";
        private const string NoTextDefault = "<NoText>";

        public Task ExportAsync(Document document, IRenderer renderer, Stream stream, PropertiesBag properties = null)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNull(stream, nameof(stream));

            bool useColors = properties != null && properties.HasProperty("HasColors") && properties["HasColors"].ToBoolean(CultureInfo.InvariantCulture);

            string noTextPlaceholder =
                properties != null &&
                properties.HasProperty("NoTextPlaceholder") ? 
                properties.HasProperty("NoTextPlaceholder").ToString() :
                NoTextDefault;

            return WriteOutlineAsync(document, renderer, stream, useColors, noTextPlaceholder);
        }

        public Task WriteOutlineAsync(Document document, IRenderer renderer, Stream stream, bool useColors, string noTextPlaceholder)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNullOrEmpty(noTextPlaceholder, nameof(noTextPlaceholder));

            return Task.Run(() =>
            {
                XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { OmitXmlDeclaration = true });

                xmlWriter.WriteStartElement("div");

                WriteNode(xmlWriter, document.Root, renderer, "1.4em", useColors, noTextPlaceholder);

                List<Node> children = document.Root.LeftChildren.Union(document.Root.RightChildren).ToList();

                if (children.Count > 0)
                {
                    xmlWriter.WriteStartElement("ul");
                    xmlWriter.WriteAttributeString("style", ListStyle);

                    foreach (Node node in children)
                    {
                        xmlWriter.WriteStartElement("li");
                        xmlWriter.WriteAttributeString("style", ListItemStyle);

                        WriteNodeWithChildren(xmlWriter, node, renderer, "1.2em", useColors, noTextPlaceholder);

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
            });
        }

        private static void WriteNodeWithChildren(XmlWriter xmlWriter, Node node, IRenderer renderer, string fontSize, bool useColors, string noTextPlaceholder)
        {
            WriteNode(xmlWriter, node, renderer, fontSize, useColors, noTextPlaceholder);

            if (node.Children.Count > 0)
            {
                xmlWriter.WriteStartElement("ul");
                xmlWriter.WriteAttributeString("style", ListStyle);

                foreach (Node child in node.Children)
                {
                    xmlWriter.WriteStartElement("li");
                    xmlWriter.WriteAttributeString("style", ListItemStyle);

                    WriteNodeWithChildren(xmlWriter, child, renderer, "1.em", useColors, noTextPlaceholder);

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }
        }

        private static void WriteNode(XmlWriter xmlWriter, NodeBase nodeBase, IRenderer renderer, string fontSize, bool useColors, string noTextPlaceholder)
        {
            string color = "#000";

            if (useColors)
            {
                LayoutThemeColor themeColor = renderer.FindColor(nodeBase);

                color = ColorsHelper.ConvertToRGBString(themeColor.Darker);
            }

            xmlWriter.WriteStartElement("span");
            xmlWriter.WriteAttributeString("style", string.Format(CultureInfo.CurrentCulture, "color:{0};font-size:{1};", color, fontSize));

            xmlWriter.WriteValue(!string.IsNullOrWhiteSpace(nodeBase.Text) ? nodeBase.Text : noTextPlaceholder);

            xmlWriter.WriteEndElement();
        }
    }
}
