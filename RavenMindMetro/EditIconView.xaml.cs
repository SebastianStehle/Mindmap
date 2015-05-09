// ==========================================================================
// EditIconView.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Controls;
using RavenMind.Model;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RavenMind
{
    public sealed partial class EditIconView : UserControl
    {
        #region Fields

        private string oldIconKey;

        #endregion

        #region Properties

        public Mindmap Mindmap { get; set; }

        #endregion

        #region Constructors

        public EditIconView()
        {
            InitializeComponent();
        }

        public EditIconView(Mindmap mindmap)
            : this()
        {
            Mindmap = mindmap;

            DataContext = mindmap.Document;
        }

        #endregion

        #region Methods

        private void EditIconView_Loaded(object sender, RoutedEventArgs e)
        {
            oldIconKey = ((IEnumerable<string>)IconsGrid.ItemsSource).FirstOrDefault(x => x == Mindmap.Document.SelectedNode.IconKey);

            if (oldIconKey == null)
            {
                IconsGrid.SelectedIndex = -1;
            }
            else
            {
                IconsGrid.SelectedItem = oldIconKey;
            }

            Mindmap.Document.BeginTransaction("EditIcon");
        }

        private void EditIconView_Unloaded(object sender, RoutedEventArgs e)
        {
            NodeBase selectedNode = Mindmap.Document.SelectedNode;

            if (selectedNode.IconKey != oldIconKey)
            {
                Mindmap.Document.CommitTransaction();
            }
        }

        private void RemoveIconButton_Click(object sender, RoutedEventArgs e)
        {
            Mindmap.Document.Apply(new ChangeIconKeyCommand { IconKey = null });
        }

        #endregion
    }
}
