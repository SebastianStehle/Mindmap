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
        #region Fields

        private int oldColor;

        #endregion

        #region Properties

        public Mindmap Mindmap { get; set; }

        #endregion

        #region Constructors

        public EditColorView()
        {
            InitializeComponent();
        }

        public EditColorView(Mindmap mindmap)
            : this()
        {
            Mindmap = mindmap;

            DataContext = mindmap.Document;
        }

        #endregion

        #region Methods

        private void EditColorView_Loaded(object sender, RoutedEventArgs e)
        {
            oldColor = Mindmap.Document.SelectedNode.Color;

            Mindmap.Document.BeginTransaction("Change Color");
        }

        private void EditColorView_Unloaded(object sender, RoutedEventArgs e)
        {
            NodeBase selectedNode = Mindmap.Document.SelectedNode;

            if (selectedNode.Color != oldColor)
            {
                Mindmap.Document.CommitTransaction();
            }
        }

        #endregion
    }
}
