// ==========================================================================
// NodeTransactionExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model.Utils;

namespace Hercules.Model
{
    public static class NodeTransactionExtensions
    {
        public static NodeBase AddSibilingransactional(this NodeBase node)
        {
            Node selectedNormalNode = node as Node;

            if (selectedNormalNode?.Document != null)
            {
                InsertChildCommand command = new InsertChildCommand(selectedNormalNode.Parent, null, selectedNormalNode.NodeSide);

                string tansactionName = ResourceManager.GetString("TransactionName_AddSibling");

                selectedNormalNode.Document.MakeTransaction(tansactionName, commands =>
                {
                    commands.Apply(command);
                });

                return command.Child;
            }

            return null;
        }

        public static NodeBase AddChildTransactional(this NodeBase node)
        {
            if (node?.Document != null)
            {
                InsertChildCommand command = new InsertChildCommand(node, null, NodeSide.Undefined);

                string tansactionName = ResourceManager.GetString("TransactionName_AddChild");

                node.Document.MakeTransaction(tansactionName, commands =>
                {
                    commands.Apply(command);
                });

                return command.Child;
            }

            return null;
        }

        public static void RemoveTransactional(this NodeBase node)
        {
            Node selectedNormalNode = node as Node;

            if (selectedNormalNode?.Document != null)
            {
                string tansactionName = ResourceManager.GetString("TransactionName_RemoveNode");

                node.Document.MakeTransaction(tansactionName, commands =>
                {
                    commands.Apply(new RemoveChildCommand(selectedNormalNode.Parent, selectedNormalNode));
                });
            }
        }

        public static void MoveTransactional(this NodeBase node, NodeBase target, int? index, NodeSide side)
        {
            Node selectedNormalNode = node as Node;

            if (selectedNormalNode?.Document != null)
            {
                string tansactionName = ResourceManager.GetString("TransactionName_MoveNode");

                selectedNormalNode.Document.MakeTransaction(tansactionName, commands =>
                {
                    commands.Apply(new RemoveChildCommand(selectedNormalNode.Parent, selectedNormalNode));

                    commands.Apply(new InsertChildCommand(target, index, side, selectedNormalNode));
                });
            }
        }

        public static void ToggleHullTransactional(this NodeBase node)
        {
            if (node?.Document != null)
            {
                string tansactionName = ResourceManager.GetString("TransactionName_ToggleHull");

                node.Document.MakeTransaction(tansactionName, commands =>
                {
                    commands.Apply(new ToggleHullCommand(node));
                });
            }
        }

        public static void ToggleCollapseTransactional(this NodeBase node)
        {
            if (node?.Document != null)
            {
                string transactionName = ResourceManager.GetString("TransactionName_ExpandCollapse");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ToggleCollapseCommand(node));
                });
            }
        }

        public static void ChangeShapeTransactional(this Node node, NodeShape? shape)
        {
            if (node?.Document != null && !Equals(node.Shape, shape))
            {
                string tansactionName = ResourceManager.GetString("TransactionName_ChangeShape");

                node.Document.MakeTransaction(tansactionName, commands =>
                {
                    commands.Apply(new ChangeShapeCommand(node, shape));
                });
            }
        }

        public static void ChangeTextTransactional(this NodeBase node, string text)
        {
            if (node?.Document != null && !Equals(node.Text, text))
            {
                string transactionName = ResourceManager.GetString("TransactionName_ChangeText");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ChangeTextCommand(node, text ?? string.Empty));
                });
            }
        }

        public static void ChangeColorTransactional(this NodeBase node, INodeColor color)
        {
            if (node?.Document != null && !Equals(node.Color, color))
            {
                string transactionName = ResourceManager.GetString("TransactionName_EditColor");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ChangeColorCommand(node, color));
                });
            }
        }

        public static void ChangeIconTransactional(this NodeBase node, INodeIcon icon)
        {
            if (node?.Document != null && !Equals(node.Icon, icon))
            {
                string transactionName = ResourceManager.GetString("TransactionName_EditIcon");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ChangeIconCommand(node, icon));
                });
            }
        }

        public static void ChangeIconSizeTransactional(this NodeBase node, IconSize iconSize)
        {
            if (node?.Document != null && !Equals(node.IconSize, iconSize))
            {
                string transactionName = ResourceManager.GetString("TransactionName_EditIconSize");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ChangeIconSizeCommand(node, iconSize));
                });
            }
        }

        public static void ChangeIconPositionTransactional(this NodeBase node, IconPosition iconPosition)
        {
            if (node?.Document != null && !Equals(node.IconPosition, iconPosition))
            {
                string transactionName = ResourceManager.GetString("TransactionName_EditIconPosition");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ChangeIconPositionCommand(node, iconPosition));
                });
            }
        }
    }
}
