﻿// ==========================================================================
// PopupHandler.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace SE.Metro.UI
{
    /// <summary>
    /// Manages the current popup and allows to simply show any view as popup with one method call.
    /// </summary>
    public static class PopupHandler 
    {
        private static FrameworkElement popupView;
        private static Popup popupContainer;
        private static PopupMode popupMode;
        private static Point? popupOffset;
        private static bool isWaitingToOpen;

        private enum PopupMode
        {
            LeftBottom,
            RightTop,
            Center
        }

        /// <summary>
        /// Shows the <see cref="IPopupControl"/> view at the top right side of the screen using the specified offset.
        /// </summary>
        /// <typeparam name="TView">The type of the view to show.</typeparam>
        /// <param name="view">The view to show. Cannot be null.</param>
        /// <param name="offset">The offset of the popup, relative to the top right side of the screen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="view"/> is null.</exception>
        public static void ShowPopupRightTop<TView>(TView view, Point offset) where TView : FrameworkElement
        {
            ShowPopup(view, offset, PopupMode.RightTop);
        }

        /// <summary>
        /// Creates a new view and displays it as popup at the top right side of the screen using the specified offset.
        /// </summary>
        /// <typeparam name="TView">The type of the view to show.</typeparam>
        /// <param name="offset">The offset of the popup, relative to the top right side of the screen.</param>
        public static void ShowPopupRightTop<TView>(Point offset) where TView : FrameworkElement, new()
        {
            TView view = new TView();

            ShowPopupRightTop(view, offset);
        }

        /// <summary>
        /// Shows the <see cref="IPopupControl"/> view at the bottom left side of the screen using the specified offset.
        /// </summary>
        /// <typeparam name="TView">The type of the view to show.</typeparam>
        /// <param name="view">The view to show. Cannot be null.</param>
        /// <param name="offset">The offset of the popup, relative to the bottom left side of the screen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="view"/> is null.</exception>
        /// <remarks>The popup will be repositioned when the virtual keyboard becomes visible.</remarks>
        public static void ShowPopupLeftBottom<TView>(TView view, Point offset) where TView : FrameworkElement
        {
            ShowPopup(view, offset, PopupMode.LeftBottom);
        }

        /// <summary>
        /// Creates a new view and displays it as popup at the bottom left side of the screen using the specified offset.
        /// </summary>
        /// <typeparam name="TView">The type of the view to show.</typeparam>
        /// <param name="offset">The offset of the popup, relative to the bottom left side of the screen.</param>
        /// <remarks>The popup will be repositioned when the virtual keyboard becomes visible.</remarks>
        public static void ShowPopupLeftBottom<TView>(Point offset) where TView : FrameworkElement, new()
        {
            TView view = new TView();

            ShowPopupLeftBottom(view, offset);
        }

        /// <summary>
        /// Shows the <see cref="IPopupControl"/> view at the center of the screen using the specified offset.
        /// </summary>
        /// <typeparam name="TView">The type of the view to show.</typeparam>
        /// <param name="view">The view to show. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="view"/> is null.</exception>
        /// <remarks>The popup will be repositioned when the virtual keyboard becomes visible.</remarks>
        public static void ShowPopupCenter<TView>(TView view) where TView : FrameworkElement
        {
            ShowPopup(view, null, PopupMode.Center);
        }

        /// <summary>
        /// Creates a new view and displays it as popup at the center of the screen using the specified offset.
        /// </summary>
        /// <typeparam name="TView">The type of the view to show.</typeparam>
        /// <remarks>The popup will be repositioned when the virtual keyboard becomes visible.</remarks>
        public static void ShowPopupCenter<TView>() where TView : FrameworkElement, new()
        {
            TView view = new TView();

            ShowPopupCenter(view);
        }

        private static void ShowPopup<TView>(TView view, Point? offset, PopupMode mode) where TView : FrameworkElement
        {
            Guard.NotNull(view, "view");

            popupOffset = offset;
            popupMode = mode;
            popupView = view;

            if (popupContainer != null)
            {
                isWaitingToOpen = true;

                popupContainer.IsOpen = false;
            }
            else
            {
                OpenPopup();

                Bind();
            }
        }

        private static void OpenPopup()
        {
            try
            {
                popupContainer = new Popup { Child = popupView, IsLightDismissEnabled = true };
                popupContainer.Closed += popup_Closed;
                popupContainer.IsOpen = true;

                IPopupControl popupControl = popupView as IPopupControl;

                if (popupControl != null)
                {
                    popupControl.Popup = popupContainer;
                }

                UpdatePopupOffset();
            }
            finally
            {
                isWaitingToOpen = false;
            }
        }

        private static void popup_Closed(object sender, object e)
        {
            popupContainer.Closed -= popup_Closed;

            if (isWaitingToOpen)
            {
                OpenPopup();
            }
            else
            {
                Unbind();

                popupContainer = null;
                popupView = null;
            }
        }

        private static void UpdatePopupOffset()
        {
            if (popupContainer != null)
            {
                double x = 0;
                double y = 0;

                switch (popupMode)
                {
                    case PopupMode.LeftBottom:
                        x = popupOffset.Value.X;
                        y = Window.Current.Bounds.Height - popupView.Height + popupOffset.Value.Y;
                        break;
                    case PopupMode.RightTop:
                        y = popupOffset.Value.Y;
                        x = Window.Current.Bounds.Width - popupView.Width + popupOffset.Value.X;
                        break;
                    case PopupMode.Center:
                        x = 0.5 * (Window.Current.Bounds.Width  - popupView.Width);
                        y = 0.5 * (Window.Current.Bounds.Height - popupView.Height);
                        break;
                    default:
                        break;
                }

                popupContainer.VerticalOffset = y;
                popupContainer.HorizontalOffset = x;
            }
        }

        private static void InputPane_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (popupContainer != null && popupMode == PopupMode.LeftBottom)
            {
                popupContainer.AnimateY(0, TimeSpan.FromSeconds(0.2));
            }
        }

        private static void InputPane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (popupContainer != null && popupMode == PopupMode.LeftBottom)
            {
                popupContainer.AnimateY(-args.OccludedRect.Height, TimeSpan.FromSeconds(0.2));
            }
        }

        private static void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            UpdatePopupOffset();
        }

        private static void Bind()
        {
            InputPane inputPane = InputPane.GetForCurrentView();

            if (inputPane != null)
            {
                inputPane.Showing += InputPane_Showing;
                inputPane.Hiding  += InputPane_Hiding;
            }

            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private static void Unbind()
        {
            InputPane inputPane = InputPane.GetForCurrentView();

            if (inputPane != null)
            {
                inputPane.Showing -= InputPane_Showing;
                inputPane.Hiding  -= InputPane_Hiding;
            }

            Window.Current.SizeChanged -= Current_SizeChanged;
        }
    }
}