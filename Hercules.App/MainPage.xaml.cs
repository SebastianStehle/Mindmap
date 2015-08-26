// ==========================================================================
// MainPage.xaml.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using GP.Windows.UI;
using GP.Windows.UI.Interactivity;
using Hercules.App.Modules.Editor.ViewModels;
using Hercules.Model;

namespace Hercules.App
{
    public sealed partial class MainPage
    {
        public EditorViewModel EditorViewModel
        {
            get 
            { 
                return MainGrid.DataContext as EditorViewModel; 
            }
        }
        
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InputPane inputPane = InputPane.GetForCurrentView();

            if (inputPane != null)
            {
                inputPane.Showing += InputPane_Showing;
                inputPane.Hiding += InputPane_Hiding;
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            InputPane inputPane = InputPane.GetForCurrentView();

            if (inputPane != null)
            {
                inputPane.Showing -= InputPane_Showing;
                inputPane.Hiding -= InputPane_Hiding;
            }

            base.OnNavigatedFrom(e);
        }
        
        private void InputPane_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (EditorViewModel.Document != null)
            {
                BottomAppBar.AnimateY(0, TimeSpan.FromSeconds(0.15));

                args.EnsuredFocusedElementInView = true;
            }
        }

        private void InputPane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (EditorViewModel.Document != null)
            {
                BottomAppBar.AnimateY(args.OccludedRect.Height * (-1), TimeSpan.FromSeconds(0.2));

                args.EnsuredFocusedElementInView = true;
            }
        }

        private void RemoveButton_Invoking(object sender, CommandInvokingEventHandler e)
        {
            TextBox textBox = FocusManager.GetFocusedElement() as TextBox;

            if (textBox != null)
            {
                e.Handled = true;
            }
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

        private void BottomBar_Closed(object sender, object e)
        {
            if (BottomAppBar != null)
            {
                BottomAppBar.IsOpen = true;
            }
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null && Mindmap.Renderer != null)
            {
                EditIconView view = new EditIconView
                {
                    Document = Mindmap.Document,
                    Renderer = Mindmap.Renderer
                };

                PopupHandler.ShowPopupLeftBottom(view, new Point(10, 80));
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (Mindmap.Document != null && Mindmap.Renderer != null)
            {
                EditColorView view = new EditColorView
                {
                    Document = Mindmap.Document,
                    Renderer = Mindmap.Renderer
                };

                PopupHandler.ShowPopupLeftBottom(view, new Point(70, 80));
            }
        }
    }
}
