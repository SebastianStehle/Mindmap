// ==========================================================================
// MindmapsView.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using GP.Windows.UI;
using Hercules.App.Modules.Mindmaps.ViewModels;

namespace Hercules.App.Modules.Mindmaps.Views
{
    public sealed partial class MindmapsView
    {
        public MindmapsView()
        {
            InitializeComponent();
        }

        private async void MindmapsPage_Loaded(object sender, RoutedEventArgs e)
        {
            MindmapsViewModel viewModel = DataContext as MindmapsViewModel;

            if (viewModel != null)
            {
                await viewModel.LoadAsync();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Control senderElement = (Control)sender;

            PopupHandler.ShowPopupLeftTop(new EnterNameView { DataContext = senderElement.DataContext }, new Point(50, 0));
        }

        private void MindmapItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;

            FlyoutBase.ShowAttachedFlyout(senderElement);

            e.Handled = true;
        }

        private void MindmapItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;

            FlyoutBase.ShowAttachedFlyout(senderElement);

            e.Handled = true;
        }
    }
}
