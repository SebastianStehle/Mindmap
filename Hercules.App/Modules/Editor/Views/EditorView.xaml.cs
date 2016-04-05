// ==========================================================================
// EditorView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Hercules.App.Modules.Editor.ViewModels;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class EditorView
    {
        public EditorViewModel ViewModel
        {
            get { return (EditorViewModel)DataContext; }
        }

        public EditorView()
        {
            InitializeComponent();
        }

        private void EditTextCommand_Invoked(object sender, RoutedEventArgs e)
        {
            Mindmap.EditText();
        }
    }
}
