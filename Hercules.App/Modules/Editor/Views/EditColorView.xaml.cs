// ==========================================================================
// EditColorView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls;
using Hercules.App.Components.Implementations;
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
            ColorsGrid.ItemsSource = Renderer.Resources.Colors;

            NodeBase selectedNode = Document.SelectedNode;

            oldColor = selectedNode.Color;
            oldIndex = Document.UndoRedoManager.Index;

            ColorsGrid.SelectedIndex = oldColor;
        }

        private void Change(int index)
        {
            NodeBase selectedNode = Document.SelectedNode;

            if (selectedNode != null)
            {
                Document.UndoRedoManager.RevertTo(oldIndex);

                if (index != oldColor)
                {
                    string tansactionName = ResourceManager.GetString("EditColorTransactionName");

                    Document.MakeTransaction(tansactionName, commands =>
                    {
                        commands.Apply(new ChangeColorCommand(selectedNode, index));
                    });
                }
            }
        }
    }
}
