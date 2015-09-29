// ==========================================================================
// MainPage.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Graphics.Printing;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using GP.Windows.UI.Interactivity;
using Hercules.App.Components.Implementations;
using Hercules.App.Modules.Mindmaps.ViewModels;
using Hercules.Model;

namespace Hercules.App
{
    public sealed partial class MainPage
    {
        private IPrintDocumentSource printDocument;

        public MainPage()
        {
            InitializeComponent();

            var themeDarkBrush = (SolidColorBrush)Application.Current.Resources.MergedDictionaries[1]["ThemeDarkBrush"];
            var themeLightBrush = (SolidColorBrush)Application.Current.Resources.MergedDictionaries[1]["ThemeLightBrush"];

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

        private async void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            MindmapsViewModel viewModel = (MindmapsViewModel)((FrameworkElement)sender).DataContext;

            await viewModel.LoadAsync();
        }

        private void RemoveButton_Invoking(object sender, CommandInvokingEventHandler e)
        {
            TextBox textBox = FocusManager.GetFocusedElement() as TextBox;

            if (textBox != null)
            {
                e.Handled = true;
            }
        }

        private async void PrintItem_Click(object sender, RoutedEventArgs e)
        {
            if (printDocument != null)
            {
                IDisposable disposable = printDocument as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            if (Mindmap.Document != null && Mindmap.Renderer != null)
            {
                printDocument = Mindmap.Renderer.Print();

                PrintManager printManager = PrintManager.GetForCurrentView();

                printManager.PrintTaskRequested += PrintManager_PrintTaskRequested;
                try
                {
                    await PrintManager.ShowPrintUIAsync();
                }
                finally
                {
                    printManager.PrintTaskRequested -= PrintManager_PrintTaskRequested;
                }
            }
        }

        private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            string title = ResourceManager.GetString("Print_Title");

            args.Request.CreatePrintTask(title, a =>
            {
                a.SetSource(printDocument);
            });
        }

        private void MoveLeftCommand_Invoked(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null)
            {
                Mindmap.Document.SelectLeftOfSelectedNode();
                Mindmap.Renderer.Invalidate();
            }
        }

        private void MoveRightCommand_Invoked(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null)
            {
                Mindmap.Document.SelectRightOfSelectedNode();
                Mindmap.Renderer.Invalidate();
            }
        }

        private void MoveTopCommand_Invoked(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null)
            {
                Mindmap.Document.SelectedTopOfSelectedNode();
                Mindmap.Renderer.Invalidate();
            }
        }

        private void MoveBottomCommand_Invoked(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null)
            {
                Mindmap.Document.SelectedBottomOfSelectedNode();
                Mindmap.Renderer.Invalidate();
            }
        }

        private void EditTextCommand_Invoked(object sender, RoutedEventArgs e)
        {
            Mindmap.EditText();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)((FrameworkElement)sender).DataContext);
        }
    }
}
