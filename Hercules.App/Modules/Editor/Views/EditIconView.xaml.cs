// ==========================================================================
// EditIconView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hercules.Model;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class EditIconView
    {
        private bool hasChange;

        public EditIconView()
        {
            InitializeComponent();

            IconsGrid.SelectionChanged += IconsGrid_SelectionChanged;
        }

        public override void OnOpened()
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                if (selectedNode.IconKey == null)
                {
                    IconsGrid.SelectedIndex = -1;
                }
                else
                {
                    IconsGrid.SelectedItem = selectedNode.IconKey;
                }
            }

            hasChange = false;
        }

        private void IconsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = IconsGrid.SelectedItem as string;

            Change(selected);
        }

        private void RemoveIconButton_Click(object sender, RoutedEventArgs e)
        {
            Change(null);
        }

        private void Change(string selected)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                if (selected != selectedNode.IconKey)
                {
                    if (hasChange && Document.UndoRedoManager.IsLastCommand<ChangeIconKeyCommand>(x => x.Node.Id == selectedNode.Id))
                    {
                        Document.UndoRedoManager.Revert();
                    }

                    if (selected != selectedNode.IconKey)
                    {
                        selectedNode.ChangeIconKeyTransactional(selected);

                        hasChange = true;
                    }
                }
            }
        }
    }
}
