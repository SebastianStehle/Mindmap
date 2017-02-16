// ==========================================================================
// StylesReader.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Hercules.Model.ExImport.Formats.XMind
{
    internal static class StylesReader
    {
        private static readonly Regex ColorRegex = new Regex("^#[0-9A-F]{6}$", RegexOptions.Compiled);

        public static void ReadStyles(XDocument mapStyles, IDictionary<string, XMindStyle> stylesById)
        {
            var nodeStyles = mapStyles.Root.Element(Namespaces.Styles("styles"));

            if (nodeStyles == null)
            {
                return;
            }

            foreach (var style in nodeStyles.Elements(Namespaces.Styles("style")))
            {
                var id = style.AttributeValue("id");

                if (string.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                if (!style.IsAttributeEquals("type", "topic"))
                {
                    continue;
                }

                var properties = style.Element(Namespaces.Styles("topic-properties"));

                if (properties == null)
                {
                    continue;
                }

                var fillString = properties.AttributeValue(Namespaces.SVG("fill"));

                if (string.IsNullOrWhiteSpace(fillString) || !ColorRegex.IsMatch(fillString))
                {
                    continue;
                }

                int color;

                if (int.TryParse(fillString.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color))
                {
                    stylesById[id] = new XMindStyle { Color = color };
                }
            }
        }
    }
}
