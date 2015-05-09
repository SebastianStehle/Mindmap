// ==========================================================================
// EditColorView.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Controls;
using RavenMind.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RavenMind
{
    public sealed partial class EditColorView : UserControl
    {
        private int oldColor;
        private int oldIndex;

        public Mindmap Mindmap { get; set; }
        
        public EditColorView()
        {
            InitializeComponent();

            ColorsGrid.SelectionChanged += ColorsGrid_SelectionChanged;
        }

        public EditColorView(Mindmap mindmap)
            : this()
        {
            Mindmap = mindmap;

            DataContext = mindmap.Document;
        }

        private void ColorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = (int)ColorsGrid.SelectedItem;

            Change(selected);
        }

        private void EditColorView_Loaded(object sender, RoutedEventArgs e)
        {
            NodeBase selectedNode = Mindmap.Document.SelectedNode;

            oldColor = selectedNode.Color;
            oldIndex = Mindmap.Document.UndoRedoManager.Index;

            ColorsGrid.SelectedItem = oldIndex;
        }

        private void Change(int selected)
        {
            NodeBase selectedNode = Mindmap.Document.SelectedNode;

            if (selectedNode != null)
            {
                Mindmap.Document.UndoRedoManager.RevertTo(oldIndex);

                if (selected != oldColor)
                {
                    Mindmap.Document.MakeTransaction("EditColor", c =>
                    {
                        c.Apply(new ChangeColorCommand(selectedNode, selected));
                    });
                }
            }
        }
    }
}
