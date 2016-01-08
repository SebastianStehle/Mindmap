// ==========================================================================
// MainPage.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using GP.Windows.UI;

namespace Hercules.App
{
    public sealed partial class MainPage
    {
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
            SolidColorBrush themeDarkBrush = VisualTreeExtensions.LoadFromAppResource<SolidColorBrush>("ThemeDarkBrush");
            SolidColorBrush themeLightBrush = VisualTreeExtensions.LoadFromAppResource<SolidColorBrush>("ThemeLightBrush");

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
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

        private void Toolbars_ListButtonClicked(object sender, EventArgs e)
        {
            OuterSplitView.IsPaneOpen = !OuterSplitView.IsPaneOpen;

            if (OuterSplitView.IsPaneOpen)
            {
                if (InnerSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
                {
                    InnerSplitView.IsPaneOpen = false;
                }

                MindmapsContainer.Focus(FocusState.Programmatic);
            }
        }

        private void Toolbars_PropertiesButtonClicked(object sender, EventArgs e)
        {
            InnerSplitView.IsPaneOpen = !InnerSplitView.IsPaneOpen;

            if (InnerSplitView.IsPaneOpen)
            {
                if (OuterSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
                {
                    OuterSplitView.IsPaneOpen = false;
                }
            }
        }
    }
}
