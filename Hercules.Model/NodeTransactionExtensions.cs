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
        public static void AddChildTransactional(this NodeBase node)
        {
            if (node?.Document != null)
            {
                string tansactionName = ResourceManager.GetString("TransactionName_AddChild");

                node.Document.MakeTransaction(tansactionName, commands =>
                {
                    commands.Apply(new InsertChildCommand(node, null, NodeSide.Undefined));
                });
            }
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

        public static void AddSibilingransactional(this NodeBase node)
        {
            Node selectedNormalNode = node as Node;

            if (selectedNormalNode?.Document != null)
            {
                string tansactionName = ResourceManager.GetString("TransactionName_AddSibling");

                selectedNormalNode.Document.MakeTransaction(tansactionName, commands =>
                {
                    commands.Apply(new InsertChildCommand(selectedNormalNode.Parent, null, selectedNormalNode.NodeSide));
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

                    commands.Apply(new InsertChildCommand(target, index, target.NodeSide, selectedNormalNode));
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

        public static void ChangeTextTransactional(this NodeBase node, string text)
        {
            if (node?.Document != null)
            {
                string transactionName = ResourceManager.GetString("TransactionName_ChangeText");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ChangeTextCommand(node, text ?? string.Empty, true));
                });
            }
        }

        public static void ChangeColorTransactional(this NodeBase node, int color)
        {
            if (node?.Document != null)
            {
                string transactionName = ResourceManager.GetString("TransactionName_EditColor");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ChangeColorCommand(node, color));
                });
            }
        }

        public static void ChangeIconKeyTransactional(this NodeBase node, string icon)
        {
            if (node?.Document != null)
            {
                string transactionName = ResourceManager.GetString("TransactionName_EditIcon");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ChangeIconKeyCommand(node, icon));
                });
            }
        }
    }
}
