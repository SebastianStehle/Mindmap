// ==========================================================================
// NodeId.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    public sealed class NodeId : IEquatable<NodeId>, IXmlSerializable
    {
        private readonly Guid id;
        private NodeBase linkedNode;

        public Guid Id
        {
            get
            {
                return id;
            }
        }

        public NodeBase LinkedNode
        {
            get
            {
                return linkedNode;
            }
        }

        public NodeId(Guid id)
        {
            this.id = id;
        }

        public NodeId()
        {
            this.id = Guid.NewGuid();
        }

        public NodeId(NodeBase node)
        {
            id = node.NodeId.Id;

            this.linkedNode = node;
        }

        internal void Link(NodeBase node)
        {
            linkedNode = node;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Equals(NodeId other)
        {
            return other != null && other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            return obj is NodeId && Equals((NodeId)obj);
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public static implicit operator NodeId(Guid id)
        {
            return new NodeId(id);
        }

        public static implicit operator NodeId(NodeBase node)
        {
            return new NodeId(node);
        }

        public void ReadXml(XmlReader reader)
        {
            string id = reader.GetAttribute("Id");

            id = Guid.Parse(id);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id.ToString());
        }
    }
}
