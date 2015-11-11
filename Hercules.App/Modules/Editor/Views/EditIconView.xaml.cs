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
                if (selectedNode.Icon == null)
                {
                    IconsGrid.SelectedIndex = -1;
                }
                else
                {
                    KeyIcon integratedIcon = (KeyIcon)selectedNode.Icon;

                    IconsGrid.SelectedItem = integratedIcon.Key;
                }
            }

            hasChange = false;
        }

        private void IconsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = IconsGrid.SelectedItem as string;

            Change(selected != null ? new KeyIcon(selected) : null);
        }

        private void RemoveIconButton_Click(object sender, RoutedEventArgs e)
        {
            Change(null);
        }

        private void Change(INodeIcon selected)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                if (!ReferenceEquals(selected, selectedNode.Icon) || (selected != null && selectedNode.Icon != null && !selected.Equals(selectedNode.Icon)))
                {
                    if (hasChange && Document.UndoRedoManager.IsLastCommand<ChangeIconCommand>(x => x.Node.Id == selectedNode.Id))
                    {
                        Document.UndoRedoManager.Revert();
                    }

                    if (!ReferenceEquals(selected, selectedNode.Icon) || (selected != null && selectedNode.Icon != null && !selected.Equals(selectedNode.Icon)))
                    {
                        selectedNode.ChangeIconKeyTransactional(selected);

                        hasChange = true;
                    }
                }
            }
        }
    }
}
