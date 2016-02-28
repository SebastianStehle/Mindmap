// ==========================================================================
// ChangeCheckableModeCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class ChangeCheckableModeCommand : CommandBase<NodeBase>
    {
        private const string PropertyCheckableMode = "CheckableMode";
        private readonly CheckableMode newCheckableMode;
        private CheckableMode oldCheckableMode;

        public CheckableMode NewCheckableMode
        {
            get { return newCheckableMode; }
        }

        public ChangeCheckableModeCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            CheckableMode value;

            int intValue;

            if (properties.TryParseEnum(PropertyCheckableMode, out value))
            {
                newCheckableMode = value;
            }
            else if (properties.TryParseInt32(PropertyCheckableMode, out intValue) && Enum.IsDefined(typeof(NodeSide), intValue))
            {
                newCheckableMode = (CheckableMode)intValue;
            }
            else
            {
                newCheckableMode = CheckableMode.Default;
            }
        }

        public ChangeCheckableModeCommand(NodeBase node, CheckableMode newCheckableMode)
            : base(node)
        {
            this.newCheckableMode = newCheckableMode;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set(PropertyCheckableMode, newCheckableMode.ToString());

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldCheckableMode = Node.CheckableMode;

            Node.ChangeCheckableMode(newCheckableMode);

            if (isRedo)
            {
                Node.Select();
            }
        }

        protected override void Revert()
        {
            Node.ChangeCheckableMode(oldCheckableMode);
            Node.Select();
        }
    }
}
