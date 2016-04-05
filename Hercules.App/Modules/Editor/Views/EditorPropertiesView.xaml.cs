// ==========================================================================
// EditorPropertiesView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Hercules.App.Modules.Editor.ViewModels;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class EditorPropertiesView
    {
        public EditorPropertiesViewModel ViewModel
        {
            get { return (EditorPropertiesViewModel)DataContext;  }
        }

        public EditorPropertiesView()
        {
            InitializeComponent();
        }

        private void EditorPropertiesView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Root.Width = e.NewSize.Width;
        }
    }
}