// ==========================================================================
// MindmapFlyoutView.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using Hercules.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hercules.Model.Rendering.Win2D;

namespace Hercules.App.Controls
{
    public abstract class MindmapFlyoutView : UserControl
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(Document), typeof(MindmapFlyoutView), new PropertyMetadata(null));
        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.Register("Renderer", typeof(Win2DRenderer), typeof(MindmapFlyoutView), new PropertyMetadata(null));
        public Win2DRenderer Renderer
        {
            get { return (Win2DRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        protected MindmapFlyoutView()
        {
        }

        public virtual void OnOpening()
        {
        }

        public virtual void OnOpened()
        {
        }

        public virtual void OnClosed()
        {
        }
    }
}
