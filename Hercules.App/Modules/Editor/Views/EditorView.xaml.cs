// ==========================================================================
// EditorView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class EditorView
    {
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
