// ==========================================================================
// MainPage.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using GP.Utils.UI;
using Hercules.App.Modules.Mindmaps.ViewModels;

namespace Hercules.App
{
    public sealed partial class MainPage
    {
        public MindmapsViewModel ViewModel
        {
            get { return (MindmapsViewModel)DataContext; }
        }

        public MainPage()
        {
            InitializeComponent();

            if (!DesignMode.DesignModeEnabled)
            {
                ApplyThemeColors();
            }
        }

        private static void ApplyThemeColors()
        {
            var themeDarkBrush = VisualTreeExtensions.LoadFromAppResource<SolidColorBrush>("ThemeDarkBrush");
            var themeLightBrush = VisualTreeExtensions.LoadFromAppResource<SolidColorBrush>("ThemeLightBrush");

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = themeDarkBrush.Color;
            titleBar.ForegroundColor = Colors.White;
            titleBar.InactiveBackgroundColor = themeDarkBrush.Color;
            titleBar.InactiveForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = themeDarkBrush.Color;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = themeDarkBrush.Color;
            titleBar.ButtonInactiveForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = themeLightBrush.Color;
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonPressedBackgroundColor = themeLightBrush.Color;
            titleBar.ButtonPressedForegroundColor = Colors.Black;
        }

        private void Toolbars_ListButtonClicked(object sender, RoutedEventArgs e)
        {
            OuterSplitView.IsPaneOpen = !OuterSplitView.IsPaneOpen;

            if (!OuterSplitView.IsPaneOpen)
            {
                return;
            }

            if (InnerSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                InnerSplitView.IsPaneOpen = false;
            }

            MindmapsContainer.Focus(FocusState.Programmatic);
        }

        private void Toolbars_PropertiesButtonClicked(object sender, RoutedEventArgs e)
        {
            InnerSplitView.IsPaneOpen = !InnerSplitView.IsPaneOpen;

            if (!InnerSplitView.IsPaneOpen)
            {
                return;
            }

            if (OuterSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                OuterSplitView.IsPaneOpen = false;
            }
        }
    }
}
