// ==========================================================================
// ContentWriter.cs
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

namespace Hercules.Model.ExImport.Formats.XMind
{
    public static class ContentWriter
    {
        public static void WriteContent(Document document, XDocument content)
        {
            string timestamp = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture);

            var allChildren = document.Root.RightChildren.Union(document.Root.LeftChildren).ToList();

            XElement root =
                new XElement(Namespaces.Content("xmap-content"),
                    new XAttribute("version", "2.0"),
                    new XAttribute("timestamp", timestamp),
                    new XElement(Namespaces.Content("sheet"),
                        new XAttribute("id", Guid.NewGuid()),
                        new XAttribute("timestamp", timestamp),
                            new XElement(Namespaces.Content("title"), document.Root.Text),
                                CreateTopic(timestamp, document.Root, allChildren)));

            content.Add(root);
        }

        private static XElement CreateTopic(string timestamp, NodeBase node, IReadOnlyList<Node> children)
        {
            XElement topic =
                new XElement(Namespaces.Content("topic"),
                    new XAttribute("id", node.Id),
                    new XAttribute("timestamp", timestamp),
                    new XAttribute("style-id", "s" + node.Id));

            if (node is RootNode)
            {
                topic.Add(new XAttribute("structure-class", "org.xmind.ui.map"));
            }

            WriteTitle(node, topic);
            WriteFolded(node, topic);
            WriteMarker(node, topic);
            WriteChildren(children, topic, timestamp);
            WriteChildrenBoundaries(children, topic, timestamp);

            return topic;
        }

        private static void WriteTitle(NodeBase node, XContainer topic)
        {
            if (!string.IsNullOrWhiteSpace(node.Text))
            {
                topic.Add(new XElement(Namespaces.Content("title"), node.Text));
            }
        }

        private static void WriteFolded(NodeBase node, XContainer topic)
        {
            if (node.IsCollapsed)
            {
                topic.Add(new XAttribute("branch", "folded"));
            }
        }

        private static void WriteMarker(NodeBase node, XContainer topic)
        {
            KeyIcon keyIcon = node.Icon as KeyIcon;

            if (keyIcon == null)
            {
                return;
            }

            string marker = MarkerMapping.ResolveXmind(keyIcon.Key);

            if (marker != null)
            {
                topic.Add(
                    new XElement(Namespaces.Content("marker-refs"),
                        new XElement(Namespaces.Content("marker-ref"),
                            new XAttribute("marker-id", marker))));
            }
        }

        private static void WriteChildren(IReadOnlyCollection<Node> children, XContainer topic, string timestamp)
        {
            if (children.Count <= 0)
            {
                return;
            }

            XElement topics = new XElement(Namespaces.Content("topics"), new XAttribute("type", "attached"));

            foreach (Node child in children)
            {
                topics.Add(CreateTopic(timestamp, child, child.Children));
            }

            topic.Add(new XElement(Namespaces.Content("children"), topics));
        }

        private static void WriteChildrenBoundaries(IReadOnlyList<Node> children, XContainer topic, string timestamp)
        {
            if (!children.Any(x => x.IsShowingHull))
            {
                return;
            }

            XElement boundaries = new XElement(Namespaces.Content("boundaries"));

            for (int i = 0; i < children.Count; i++)
            {
                Node child = children[i];

                if (child.IsShowingHull)
                {
                    boundaries.Add(
                        new XElement(Namespaces.Content("boundary"),
                            new XAttribute("id", Guid.NewGuid()),
                            new XAttribute("range", $"({i}, {i})"),
                            new XAttribute("timestamp", timestamp)));
                }
            }

            topic.Add(boundaries);
        }
    }
}
