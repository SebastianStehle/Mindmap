// ==========================================================================
// MindmapFlyoutPopupView.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using GP.Windows.UI;
using Hercules.Model;
using Hercules.Model.Rendering.Win2D;

namespace Hercules.App.Controls
{
    public abstract class MindmapFlyoutPopupView : UserControl, IPopupControl
    {
        public Popup Popup { get; set; }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(MindmapFlyoutPopupView), new PropertyMetadata(null));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.Register("Renderer", typeof(Win2DRenderer), typeof(MindmapFlyoutPopupView), new PropertyMetadata(null));
        public Win2DRenderer Renderer
        {
            get { return (Win2DRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        public virtual void OnOpening()
        {
        }

        public virtual void OnClosed()
        {
        }
    }
}
