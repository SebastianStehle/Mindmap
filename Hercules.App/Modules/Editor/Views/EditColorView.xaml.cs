// ==========================================================================
// EditColorView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GP.Windows.UI;
using Hercules.Model;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class EditColorView
    {
        private bool hasChange;

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
                ThemeColor themeColor = selectedNode.Color as ThemeColor;

                if (themeColor != null)
                {
                    ColorsGrid.SelectedIndex = themeColor.Index;

                    ColorsPivot.SelectedIndex = 0;
                }
                else
                {
                    ColorsPicker.SelectedColor = ColorsHelper.ConvertToColor(((ValueColor)selectedNode.Color).Color);

                    ColorsPivot.SelectedIndex = 1;
                }

                ShowHullButton.IsChecked = selectedNode.IsShowingHull;

                Node normalNode = selectedNode as Node;

                if (normalNode != null)
                {
                    if (normalNode.Shape.HasValue)
                    {
                        ShapeListBox.SelectedIndex = ((int)normalNode.Shape.Value) + 1;
                    }
                    else
                    {
                        ShapeListBox.SelectedIndex = 0;
                    }

                    ShapeListBox.IsEnabled = true;
                }
                else
                {
                    ShapeListBox.IsEnabled = false;
                }
            }
        }

        private void ColorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void ColorsPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            Update();
        }

        private void ColorsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            if (ColorsPivot.SelectedIndex == 0 && ColorsGrid.SelectedIndex >= 0)
            {
                int selectedIndex = ColorsGrid.SelectedIndex;

                Change(new ThemeColor(selectedIndex));
            }
            else if (ColorsPivot.SelectedIndex == 1)
            {
                Color selected = ColorsPicker.SelectedColor;

                Change(new ValueColor(ColorsHelper.ConvertToInt(selected)));
            }
        }

        private void Change(INodeColor newColor)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                if (!newColor.Equals(selectedNode.Color))
                {
                    if (hasChange && Document.UndoRedoManager.IsLastCommand<ChangeColorCommand>(x => x.Node.Id == selectedNode.Id))
                    {
                        Document.UndoRedoManager.Revert();
                    }

                    if (!newColor.Equals(selectedNode.Color))
                    {
                        selectedNode.ChangeColorTransactional(newColor);

                        hasChange = true;
                    }
                }
            }
        }

        private void ShowHullButton_Click(object sender, RoutedEventArgs e)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                if (hasChange == Document.UndoRedoManager.IsLastCommand<ToggleHullCommand>())
                {
                    Document.UndoRedoManager.Revert();
                }

                selectedNode.ToggleHullTransactional();

                hasChange = true;
            }
        }

        private void ShapeListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Node selectedNode = Document?.SelectedNode as Node;

            if (selectedNode != null)
            {
                if (ShapeListBox.SelectedIndex == 0)
                {
                    selectedNode.ChangeShapeTransactional(null);
                }
                else
                {
                    selectedNode.ChangeShapeTransactional((NodeShape)(ShapeListBox.SelectedIndex - 1));
                }
            }
        }
    }
}
