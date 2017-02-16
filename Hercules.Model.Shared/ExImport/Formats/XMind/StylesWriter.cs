// ==========================================================================
// StylesWriter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Xml.Linq;
using GP.Utils.Mathematics;
using Hercules.Model.Rendering;

namespace Hercules.Model.ExImport.Formats.XMind
{
    internal static class StylesWriter
    {
        public static void WriteContent(Document document, XDocument xMapStyles, IRenderer renderer)
        {
            var styles = new XElement(Namespaces.Styles("styles"));

            foreach (var node in document.Nodes)
            {
                var color = renderer.FindColor(node);

                if (color == null)
                {
                    continue;
                }

                var colorString = ColorsVectorHelper.ConvertToRGBString(color.Normal);

                var properties = new XElement(Namespaces.Styles("topic-properties"));

                if (node is RootNode || node.Parent is RootNode)
                {
                    properties.Add(new XAttribute(Namespaces.SVG("fill"), colorString));
                }
                else
                {
                    properties.Add(new XAttribute("border-line-color", colorString));
                    properties.Add(new XAttribute("line-color", colorString));
                }

                styles.Add(
                    new XElement(Namespaces.Styles("style"),
                        new XAttribute("id", "s" + node.Id),
                        new XAttribute("type", "topic"),
                        properties));
            }

            xMapStyles.Add(
                new XElement(Namespaces.Styles("xmap-styles"),
                    new XAttribute("version", "2.0"),
                    new XAttribute(XNamespace.Xmlns + "svg", Namespaces.SVGNamespace),
                    styles));
        }
    }
}
