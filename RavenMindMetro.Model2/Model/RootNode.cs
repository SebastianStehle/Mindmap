// ==========================================================================
// RootNode.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RavenMind.Model
{
    public sealed class RootNode : NodeBase
    {
        private readonly List<Node> leftChildren = new List<Node>();
        private readonly List<Node> rightChildren = new List<Node>();

        public IReadOnlyList<Node> RightChildren
        {
            get
            {
                return leftChildren;
            }
        }

        public IReadOnlyList<Node> LeftChildren
        {
            get
            {
                return rightChildren;
            }
        }

        public RootNode(Guid id, string text)
            : base(id)
        {
            Text = text;
        }

        public override RemoveChildCommand Apply(InsertChildCommand command)
        {
            PreprocessCommand(command);

            if (command.Side != NodeSide.Undefined)
            {
                if (command.Side == NodeSide.Right)
                {
                    return Add(rightChildren, command, NodeSide.Right);
                }
                else
                {
                    return Add(leftChildren, command, NodeSide.Left);
                }
            }
            else
            {
                if (rightChildren.Count <= leftChildren.Count)
                {
                    return Add(rightChildren, command, NodeSide.Right);
                }
                else
                {
                    return Add(leftChildren, command, NodeSide.Left);
                }
            }
        }

        public override InsertChildCommand Apply(RemoveChildCommand command)
        {
            PreprocessCommand(command);

            Node child = (Node)command.OldNode.LinkedNode;

            if (rightChildren.Contains(child))
            {
                return Remove(rightChildren, command);
            }
            else
            {
                return Remove(leftChildren, command);

            }
        }
    }
}
