// ==========================================================================
// EditIconView.cs
// MindmapApp Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using MindmapApp.Controls;
using MindmapApp.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MindmapApp
{
    public sealed partial class EditIconView : MindmapFlyoutView
    {
        private string oldIconKey;
        private int oldIndex;

        public EditIconView()
        {
            InitializeComponent();

            IconsGrid.SelectionChanged += IconsGrid_SelectionChanged;
        }

        private void IconsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = IconsGrid.SelectedItem as string;

            Change(selected);
        }

        public override void OnOpened()
        {
            NodeBase selectedNode = Document.SelectedNode;

            oldIconKey = selectedNode.IconKey;
            oldIndex = Document.UndoRedoManager.Index;

            if (oldIconKey == null)
            {
                IconsGrid.SelectedIndex = -1;
            }
            else
            {
                IconsGrid.SelectedItem = oldIconKey;
            }
        }

        private void RemoveIconButton_Click(object sender, RoutedEventArgs e)
        {
            Change(null);
        }

        private void Change(string selected)
        {
            NodeBase selectedNode = Document.SelectedNode;

            if (selectedNode != null)
            {
                Document.UndoRedoManager.RevertTo(oldIndex);

                if (selected != oldIconKey)
                {
                    Document.MakeTransaction("EditIcon", c =>
                    {
                        c.Apply(new ChangeIconKeyCommand(selectedNode, selected));
                    });
                }
            }
        }
    }
}
