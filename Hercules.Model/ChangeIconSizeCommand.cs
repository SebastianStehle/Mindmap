﻿// ==========================================================================
// ChangeIconSizeCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;

namespace Hercules.Model
{
    public sealed class ChangeIconSizeCommand : CommandBase
    {
        private const string PropertyKeyForIconSize = "IconSize";
        private readonly IconSize newIconSize;
        private IconSize oldIconSize;

        public IconSize NewIconSize
        {
            get { return newIconSize; }
        }

        public ChangeIconSizeCommand(PropertiesBag properties, Document document)
            : base(properties, document)
        {
            if (properties.Contains(PropertyKeyForIconSize))
            {
                try
                {
                    newIconSize = (IconSize)properties[PropertyKeyForIconSize].ToInt32(CultureInfo.InvariantCulture);
                }
                catch (InvalidCastException)
                {
                    newIconSize = IconSize.Small;
                }
            }
        }

        public ChangeIconSizeCommand(NodeBase node, IconSize newIconSize)
            : base(node)
        {
            this.newIconSize = newIconSize;
        }

        public override void Save(PropertiesBag properties)
        {
            properties.Set(PropertyKeyForIconSize, (int)newIconSize);

            base.Save(properties);
        }

        protected override void Execute(bool isRedo)
        {
            oldIconSize = Node.IconSize;

            Node.ChangeIconSize(newIconSize);
            Node.Select();
        }

        protected override void Revert()
        {
            Node.ChangeIconSize(oldIconSize);
            Node.Select();
        }
    }
}
