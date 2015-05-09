// ==========================================================================
// Node.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Linq;

namespace RavenMind.Model
{
    public class Node : NodeBase
    {
        private readonly List<Node> children = new List<Node>();

        public IReadOnlyList<Node> Children
        {
            get
            {
                return children;
            }
        }

        public Node(NodeId id)
            : base(id)
        {
        }

        public override RemoveChildCommand Apply(InsertChildCommand command)
        {
            PreprocessCommand(command);

            return Add(children, command, Parent != null ? Parent.Side : NodeSide.Undefined);
        }

        public override InsertChildCommand Apply(RemoveChildCommand command)
        {
            PreprocessCommand(command);

            return Remove(children, command);
        }
    }
}
