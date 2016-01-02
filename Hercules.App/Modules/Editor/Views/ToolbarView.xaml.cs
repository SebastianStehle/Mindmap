// ==========================================================================
// ToolbarView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;

namespace Hercules.App.Modules.Editor.Views
{
    public sealed partial class ToolbarView
    {
        public event EventHandler ListButtonClicked;

        public event EventHandler PropertiesButtonClicked;

        public ToolbarView()
        {
            InitializeComponent();
        }

        private void ListAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ListButtonClicked?.Invoke(sender, EventArgs.Empty);
        }

        private void PropertiesButton_Click(object sender, RoutedEventArgs e)
        {
            PropertiesButtonClicked?.Invoke(sender, EventArgs.Empty);
        }
    }
}
