// ==========================================================================
// MainPage.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using GP.Windows.UI;

namespace Hercules.App
{
    public sealed partial class MainPage
    {
        private bool isPropertiesOpen = true;

        public MainPage()
        {
            InitializeComponent();

            ApplyThemeColors();
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

        private void ToolbarView_ListButtonClicked(object sender, EventArgs e)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;

            MindmapsContainer.Focus(FocusState.Programmatic);
        }

        private void ToolbarView_PropertiesButtonClicked(object sender, EventArgs e)
        {
            if (isPropertiesOpen)
            {
                ShowPropertiesStoryboard.Stop();
                HidePropertiesStoryboard.Begin();
            }
            else
            {
                ShowPropertiesStoryboard.Begin();
                HidePropertiesStoryboard.Stop();
            }

            isPropertiesOpen = !isPropertiesOpen;
        }
    }
}
