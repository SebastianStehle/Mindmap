// ==========================================================================
// EditColorView.cs
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
    public sealed partial class EditColorView : MindmapFlyoutView
    {
        private int oldColor;
        private int oldIndex;
        
        public EditColorView()
        {
            InitializeComponent();

            ColorsGrid.SelectionChanged += ColorsGrid_SelectionChanged;
        }

        private void ColorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = ColorsGrid.SelectedIndex;

            Change(selected);
        }

        public override void OnOpened()
        {
            ColorsGrid.ItemsSource = Theme.Colors;

            NodeBase selectedNode = Document.SelectedNode;

            oldColor = selectedNode.Color;
            oldIndex = Document.UndoRedoManager.Index;

            ColorsGrid.SelectedIndex = oldIndex;
        }

        private void Change(int index)
        {
            NodeBase selectedNode = Document.SelectedNode;

            if (selectedNode != null)
            {
                Document.UndoRedoManager.RevertTo(oldIndex);

                if (index != oldColor)
                {
                    Document.MakeTransaction("EditColor", c =>
                    {
                        c.Apply(new ChangeColorCommand(selectedNode, index));
                    });
                }
            }
        }
    }
}
