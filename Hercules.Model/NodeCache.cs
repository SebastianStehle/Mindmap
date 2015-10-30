// ==========================================================================
// NodeCache.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hercules.Model
{
    public sealed class NodeCache
    {
        private readonly Dictionary<Guid, WeakReference<NodeBase>> nodes = new Dictionary<Guid, WeakReference<NodeBase>>();

        public bool Remove(Guid id)
        {
            return nodes.Remove(id);
        }

        public void Add(NodeBase node)
        {
            Cleanup();

            nodes[node.Id] = new WeakReference<NodeBase>(node);
        }

        public NodeBase GetOrCreateNode<T>(Guid id, Func<Guid, T> factory) where T : NodeBase
        {
            Cleanup();

            NodeBase result;
            WeakReference<NodeBase> reference;

            if (!nodes.TryGetValue(id, out reference) || reference == null || !reference.TryGetTarget(out result))
            {
                result = factory(id);

                nodes.Add(id, new WeakReference<NodeBase>(result));
            }

            return result;
        }

        private void Cleanup()
        {
            foreach (KeyValuePair<Guid, WeakReference<NodeBase>> kvp in nodes.ToList())
            {
                NodeBase temp;

                if (!kvp.Value.TryGetTarget(out temp))
                {
                    nodes.Remove(kvp.Key);
                }
            }
        }
    }
}
