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
                    ColorsPicker.SelectedColor = ColorsHelper.ConvertToColor(((CustomColor)selectedNode.Color).Color);

                    ColorsPivot.SelectedIndex = 1;
                }

                ShowHullButton.IsChecked = selectedNode.IsShowingHull;
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

                Change(new CustomColor(ColorsHelper.ConvertToInt(selected)));
            }
        }

        private void Change(IColor newColor)
        {
            NodeBase selectedNode = Document?.SelectedNode;

            if (selectedNode != null)
            {
                if (!newColor.Equals(selectedNode.Color))
                {
                    if (hasChange == Document.UndoRedoManager.IsLastCommand<ChangeColorCommand>(x => x.Node.Id == selectedNode.Id))
                    {
                        Document.UndoRedoManager.RevertOnce();
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
                    Document.UndoRedoManager.RevertOnce();
                }

                selectedNode.ToggleHullTransactional();

                hasChange = true;
            }
        }
    }
}
