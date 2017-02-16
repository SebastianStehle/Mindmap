// ==========================================================================
// ContentReader.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Model.ExImport.Formats.XMind
{
    internal static class ContentReader
    {
        public static IEnumerable<ImportResult> ReadContent(XDocument content, IReadOnlyDictionary<string, XMindStyle> stylesById)
        {
            var sheets = content.Root.Elements(Namespaces.Content("sheet"));

            foreach (var sheet in sheets)
            {
                var title = sheet.Element(Namespaces.Content("title"))?.Value;

                var document = new Document(Guid.NewGuid());

                var root = sheet.Element(Namespaces.Content("topic"));

                if (root == null)
                {
                    continue;
                }

                ReadNode(root, document.Root, stylesById);

                if (string.IsNullOrWhiteSpace(title))
                {
                    title = document.Root.Text ?? "Untitled";
                }

                yield return new ImportResult(document, title);
            }
        }

        private static void ReadNode(this XElement topic, NodeBase node, IReadOnlyDictionary<string, XMindStyle> stylesById)
        {
            ReadTitle(topic, node);
            ReadStyles(topic, node, stylesById);
            ReadFolded(topic, node);
            ReadChildren(topic, node, stylesById);
            ReadMarker(topic, node);
        }

        private static void ReadMarker(this XContainer topic, NodeBase node)
        {
            var markerRefs = topic.Element(Namespaces.Content("marker-refs"));

            if (markerRefs == null)
            {
                return;
            }

            var markerId = markerRefs.Elements(Namespaces.Content("marker-ref")).Select(x => x.Attribute("marker-id")?.Value).FirstOrDefault(x => x != null);

            if (string.IsNullOrWhiteSpace(markerId))
            {
                return;
            }

            var icon = MarkerMapping.ResolveMindapp(markerId);

            if (icon != null)
            {
                node.ChangeIconTransactional(new KeyIcon(icon));
            }
        }

        private static void ReadTitle(this XContainer topic, NodeBase node)
        {
            var title = topic.ElementValue(Namespaces.Content("title"));

            if (!string.IsNullOrWhiteSpace(title))
            {
                node.ChangeTextTransactional(title);
            }
        }

        private static void ReadFolded(this XElement topic, NodeBase node)
        {
            if (topic.IsAttributeEquals("branch", "folded"))
            {
                node.ToggleCollapseTransactional();
            }
        }

        private static void ReadStyles(this XElement topic, NodeBase node, IReadOnlyDictionary<string, XMindStyle> stylesById)
        {
            var styleId = topic.AttributeValue("style-id");

            XMindStyle style;

            if (string.IsNullOrWhiteSpace(styleId) || !stylesById.TryGetValue(styleId, out style))
            {
                return;
            }

            if (style.Color >= 0)
            {
                node.ChangeColorTransactional(new ValueColor(style.Color));
            }
        }

        private static void ReadChildren(this XContainer topic, NodeBase node, IReadOnlyDictionary<string, XMindStyle> stylesById)
        {
            var children = new List<NodeBase>();

            var topics = topic.Element(Namespaces.Content("children"))?.Element(Namespaces.Content("topics"));

            if (topics != null && topics.IsAttributeEquals("type", "attached"))
            {
                foreach (var subtopic in topics.Elements(Namespaces.Content("topic")))
                {
                    var child = node.AddChildTransactional();

                    ReadNode(subtopic, child, stylesById);

                    children.Add(child);
                }
            }

            ReadBounds(topic, children);
        }

        private static void ReadBounds(this XContainer topic, IReadOnlyList<NodeBase> children)
        {
            var boundaries = topic.Element(Namespaces.Content("boundaries"));

            if (boundaries == null)
            {
                return;
            }

            foreach (var boundary in boundaries.Elements(Namespaces.Content("boundary")))
            {
                var range = boundary.AttributeValue("range");

                if (range == null)
                {
                    continue;
                }

                range = range.Trim(' ', '(', ')');

                var parts = range.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                {
                    continue;
                }

                int s;
                int e;

                if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out s) ||
                    !int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out e))
                {
                    continue;
                }

                if (s == e && s >= 0 && s <= children.Count - 1)
                {
                    children[s].ToggleHullTransactional();
                }
            }
        }
    }
}
