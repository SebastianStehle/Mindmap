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
        public EditColorView()
        {
            InitializeComponent();

            ColorsGrid.SelectionChanged += ColorsGrid_SelectionChanged;
        }

        public override void OnOpened()
        {
            NodeBase selectedNode = Document?.SelectedNode;

            ColorsGrid.ItemsSource = Renderer.Resources.Colors;

            if (selectedNode != null)
            {
                ColorsGrid.SelectedIndex = selectedNode.Color;

                ShowHullButton.IsChecked = selectedNode.IsShowingHull;
            }
        }

        private void ColorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = ColorsGrid.SelectedIndex;

            Change(selected);
        }

        private void Change(int colorIndex)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                if (colorIndex != selectedNode.Color)
                {
                    selectedNode.ChangeColorTransactional(colorIndex);
                }
            }
        }

        private void ShowHullButton_Click(object sender, RoutedEventArgs e)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                selectedNode.ToggleHullTransactional();
            }
        }
    }
}
