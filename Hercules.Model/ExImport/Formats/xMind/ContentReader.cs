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
using System.Xml.Linq;

namespace Hercules.Model.ExImport.Formats.XMind
{
    internal static class ContentReader
    {
        public static IEnumerable<ImportResult> ReadContent(XDocument content, IReadOnlyDictionary<string, XMindStyle> stylesById)
        {
            IEnumerable<XElement> sheets = content.Root.Elements(Namespaces.Content("sheet"));

            foreach (XElement sheet in sheets)
            {
                string title = sheet.Element(Namespaces.Content("title"))?.Value;

                if (!string.IsNullOrWhiteSpace(title))
                {
                    Document document = new Document(Guid.NewGuid());

                    XElement root = sheet.Element(Namespaces.Content("topic"));

                    if (root != null)
                    {
                        ReadNode(root, document.Root, stylesById);
                    }

                    yield return new ImportResult(document, title);
                }
            }
        }

        private static void ReadNode(XElement topic, NodeBase node, IReadOnlyDictionary<string, XMindStyle> stylesById)
        {
            List<NodeBase> children = new List<NodeBase>();

            ReadTitle(topic, node);
            ReadStyle(topic, node, stylesById);
            ReadChilds(topic, node, stylesById, children);
            ReadBounds(topic, children);
            ReadCollapse(topic, node);
        }

        private static void ReadTitle(XContainer topic, NodeBase node)
        {
            string title = topic.ElementValue(Namespaces.Content("title"));

            if (!string.IsNullOrWhiteSpace(title))
            {
                node.ChangeTextTransactional(title);
            }
        }

        private static void ReadChilds(XContainer topic, NodeBase node, IReadOnlyDictionary<string, XMindStyle> stylesById, ICollection<NodeBase> children)
        {
            XElement topics = topic.Element(Namespaces.Content("children"))?.Element(Namespaces.Content("topics"));

            if (topics != null && topics.IsAttributeEquals("type", "attached"))
            {
                foreach (XElement subtopic in topics.Elements(Namespaces.Content("topic")))
                {
                    NodeBase child = node.AddChildTransactional();

                    ReadNode(subtopic, child, stylesById);

                    children.Add(child);
                }
            }
        }

        private static void ReadStyle(XElement topic, NodeBase node, IReadOnlyDictionary<string, XMindStyle> stylesById)
        {
            string styleId = topic.AttributeValue("style-id");

            XMindStyle style;

            if (!string.IsNullOrWhiteSpace(styleId) && stylesById.TryGetValue(styleId, out style))
            {
                if (style.Color >= 0)
                {
                    node.ChangeColorTransactional(new CustomColor(style.Color));
                }
            }
        }

        private static void ReadBounds(XContainer topic, IReadOnlyList<NodeBase> children)
        {
            XElement boundaries = topic.Element(Namespaces.Content("boundaries"));

            if (boundaries != null)
            {
                foreach (XElement boundary in boundaries.Elements(Namespaces.Content("boundary")))
                {
                    string range = boundary.AttributeValue("range");

                    if (range != null)
                    {
                        range = range.Trim(' ', '(', ')');

                        string[] parts = range.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 2)
                        {
                            int s;
                            int e;

                            if (int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out s) &&
                                int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out e))
                            {
                                if (s == e && s >= 0 && s <= children.Count - 1)
                                {
                                    children[s].ToggleHullTransactional();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ReadCollapse(XElement topic, NodeBase node)
        {
            if (topic.IsAttributeEquals("branch", "folded"))
            {
                node.ToggleCollapseTransactional();
            }
        }
    }
}
