// ==========================================================================
// EditIconView.cs
// MindmapApp Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using MindmapApp.Controls;
using MindmapApp.Model;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MindmapApp
{
    public sealed partial class EditIconView : UserControl
    {
        private string oldIconKey;
        private int oldIndex;

        public Mindmap Mindmap { get; set; }

        public EditIconView()
        {
            InitializeComponent();

            IconsGrid.SelectionChanged += IconsGrid_SelectionChanged;
        }

        public EditIconView(Mindmap mindmap)
            : this()
        {
            Mindmap = mindmap;

            DataContext = mindmap.Document;
        }

        private void IconsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = IconsGrid.SelectedItem as string;

            Change(selected);
        }

        private void EditIconView_Loaded(object sender, RoutedEventArgs e)
        {
            NodeBase selectedNode = Mindmap.Document.SelectedNode;

            oldIconKey = selectedNode.IconKey;
            oldIndex = Mindmap.Document.UndoRedoManager.Index;

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
            NodeBase selectedNode = Mindmap.Document.SelectedNode;

            if (selectedNode != null)
            {
                Mindmap.Document.UndoRedoManager.RevertTo(oldIndex);

                if (selected != oldIconKey)
                {
                    Mindmap.Document.MakeTransaction("EditIcon", c =>
                    {
                        c.Apply(new ChangeIconKeyCommand(selectedNode, selected));
                    });
                }
            }
        }
    }
}
