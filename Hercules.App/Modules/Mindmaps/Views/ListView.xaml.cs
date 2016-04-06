// ==========================================================================
// ListView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Hercules.App.Modules.Mindmaps.ViewModels;

// ReSharper disable InvertIf

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class ListView
    {
        public MindmapsViewModel ViewModel
        {
            get { return (MindmapsViewModel)DataContext; }
        }

        public ListView()
        {
            InitializeComponent();
        }

        private void Mindmap_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void Mindmap_RenameClicked(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)((FrameworkElement)sender).DataContext);
        }
    }
}
