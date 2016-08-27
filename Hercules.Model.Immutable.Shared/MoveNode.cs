using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.Model
{
    public sealed class MoveNode : NodeAction
    {
        private readonly Guid parentId;
        private readonly int? index;
        private readonly NodeSide side;

        public Guid ParentId
        {
            get { return parentId; }
        }

        public int? Index
        {
            get { return index; }
        }

        public NodeSide Side
        {
            get { return side; }
        }

        public MoveNode(Guid nodeId, Guid parentId, int? index = null, NodeSide side = NodeSide.Auto)
            : base(nodeId)
        {
            this.parentId = parentId;
            this.index = index;
            this.side = side;
        }
    }
}
