// ==========================================================================
// HtmlOutlineGenerator.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using GP.Windows;
using GP.Windows.UI;
using Hercules.Model.Layouting;

namespace Hercules.Model.Export.Html
{
    public sealed class HtmlOutlineGenerator : IOutlineGenerator
    {
        private const string ULStyle = "padding-left:18px;";
        private const string LIStyle = "padding-top:4px;padding-bottom:4px;";

        public string GenerateOutlineToString(Document document, IRenderer renderer, bool useColors, string noTextPlaceholder)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                GenerateOutline(document, renderer, memoryStream, true, noTextPlaceholder);
                
                memoryStream.Position = 0;

                byte[] buffer = memoryStream.ToArray();
                
                return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
        }

        public void GenerateOutline(Document document, IRenderer renderer, Stream stream, bool useColors, string noTextPlaceholder)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNullOrEmpty(noTextPlaceholder, nameof(noTextPlaceholder));

            XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { OmitXmlDeclaration = true });

            xmlWriter.WriteStartElement("div");

            WriteNode(xmlWriter, document.Root, renderer, "1.4em", useColors, noTextPlaceholder);

            List<Node> children = document.Root.LeftChildren.Union(document.Root.RightChildren).ToList();

            if (children.Count > 0)
            {
                xmlWriter.WriteStartElement("ul");
                xmlWriter.WriteAttributeString("style", ULStyle);

                foreach (Node node in children)
                {
                    xmlWriter.WriteStartElement("li");
                    xmlWriter.WriteAttributeString("style", LIStyle);

                    WriteNodeWithChildren(xmlWriter, node, renderer, "1.2em", useColors, noTextPlaceholder);

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
        }

        private static void WriteNodeWithChildren(XmlWriter xmlWriter, Node node, IRenderer renderer, string fontSize, bool useColors, string noTextPlaceholder)
        {
            WriteNode(xmlWriter, node, renderer, fontSize, useColors, noTextPlaceholder);

            if (node.Children.Count > 0)
            {
                xmlWriter.WriteStartElement("ul");
                xmlWriter.WriteAttributeString("style", ULStyle);

                foreach (Node child in node.Children)
                {
                    xmlWriter.WriteStartElement("li");
                    xmlWriter.WriteAttributeString("style", LIStyle);

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
                ThemeColor themeColor = renderer.FindColor(nodeBase);

                color = ColorsHelper.ConvertToRGBString(themeColor.Dark);
            }

            xmlWriter.WriteStartElement("span");
            xmlWriter.WriteAttributeString("style", string.Format(CultureInfo.CurrentCulture, "color:{0};font-size:{1};", color, fontSize));

            xmlWriter.WriteValue(!string.IsNullOrWhiteSpace(nodeBase.Text) ? nodeBase.Text : noTextPlaceholder);

            xmlWriter.WriteEndElement();
        }
    }
}
