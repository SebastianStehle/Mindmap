// ==========================================================================
// EditColorView.xaml.cs
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
    public sealed partial class EditColorView
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
            NodeBase selectedNode = Document.SelectedNode;

            oldColor = selectedNode.Color;
            oldIndex = Document.UndoRedoManager.Index;

            ColorsGrid.ItemsSource = Renderer.Resources.Colors;
            ColorsGrid.SelectedIndex = oldColor;

            ShowHullButton.IsChecked = selectedNode.IsShowingHull;
        }

        private void Change(int index)
        {
            NodeBase selectedNode = Document.SelectedNode;

            if (selectedNode != null)
            {
                Document.UndoRedoManager.RevertTo(oldIndex);

                if (index != oldColor)
                {
                    selectedNode.ChangeColorTransactional(index);

                    if (ShowHullButton.IsChecked == true && !selectedNode.IsShowingHull)
                    {
                        selectedNode.ToggleHullTransactional();
                    }
                }
            }
        }

        private void ShowHullButton_Click(object sender, RoutedEventArgs e)
        {
            NodeBase selectedNode = Document.SelectedNode;

            if (selectedNode != null)
            {
                selectedNode.ToggleHullTransactional();
            }
        }
    }
}
