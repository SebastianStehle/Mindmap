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

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class ListView
    {
        public ListView()
        {
            InitializeComponent();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            MindmapList.Focus(FocusState.Programmatic);
        }

        private void Mindmap_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void Mindmap_RenameClicked(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)((FrameworkElement)sender).DataContext);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MindmapsViewModel viewModel = (MindmapsViewModel)DataContext;

            await viewModel.LoadAsync();
        }
    }
}
