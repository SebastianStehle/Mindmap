// ==========================================================================
// EditIconView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hercules.App.Components.Implementations;
using Hercules.Model;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class EditIconView
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
                    string tansactionName = ResourceManager.GetString("TransactionName_EditIcon");

                    Document.MakeTransaction(tansactionName, commands =>
                    {
                        commands.Apply(new ChangeIconKeyCommand(selectedNode, selected));
                    });
                }
            }
        }
    }
}
