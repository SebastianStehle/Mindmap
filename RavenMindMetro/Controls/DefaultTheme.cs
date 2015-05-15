// ==========================================================================
// DefaultRenderer.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;
using Windows.UI.Xaml;

namespace RavenMind.Controls
{
    public sealed class DefaultTheme : ThemeBase
    {
        public static readonly DependencyProperty PathStyleProperty =
            DependencyProperty.Register("PathStyle", typeof(Style), typeof(DefaultTheme), new PropertyMetadata(null));
        public Style PathStyle
        {
            get { return (Style)GetValue(PathStyleProperty); }
            set { SetValue(PathStyleProperty, value); }
        }

        public static readonly DependencyProperty PathPreviewStyleProperty =
            DependencyProperty.Register("PathPreviewStyle", typeof(Style), typeof(DefaultTheme), new PropertyMetadata(null));
        public Style PathPreviewStyle
        {
            get { return (Style)GetValue(PathPreviewStyleProperty); }
            set { SetValue(PathPreviewStyleProperty, value); }
        }

        public static readonly DependencyProperty RootStyleProperty =
            DependencyProperty.Register("RootStyle", typeof(Style), typeof(DefaultTheme), new PropertyMetadata(null));
        public Style RootStyle
        {
            get { return (Style)GetValue(RootStyleProperty); }
            set { SetValue(RootStyleProperty, value); }
        }

        public static readonly DependencyProperty NodePreviewStyleProperty =
            DependencyProperty.Register("NodePreviewStyle", typeof(Style), typeof(DefaultTheme), new PropertyMetadata(null));
        public Style NodePreviewStyle
        {
            get { return (Style)GetValue(NodePreviewStyleProperty); }
            set { SetValue(NodePreviewStyleProperty, value); }
        }

        public static readonly DependencyProperty MainStyleProperty =
            DependencyProperty.Register("MainStyle", typeof(Style), typeof(DefaultTheme), new PropertyMetadata(null));
        public Style MainStyle
        {
            get { return (Style)GetValue(MainStyleProperty); }
            set { SetValue(MainStyleProperty, value); }
        }

        public static readonly DependencyProperty NodeStyleProperty =
            DependencyProperty.Register("NodeStyle", typeof(Style), typeof(DefaultTheme), new PropertyMetadata(null));
        public Style NodeStyle
        {
            get { return (Style)GetValue(NodeStyleProperty); }
            set { SetValue(NodeStyleProperty, value); }
        }

        public DefaultTheme()
        {
            AddColors(
                0xF7977A,
                0xF9AD81,
                0xFDC68A,
                0xFFF79A,
                0xC4DF9B,
                0xF26C4F,
                0xF68E55,
                0xFBAF5C,
                0xFFF467,
                0xACD372,
                0xA2D39C,
                0x82CA9D,
                0x7BCDC8,
                0x6ECFF6,
                0x7EA7D8,
                0x7CC576,
                0x3BB878,
                0x1ABBB4,
                0x00BFF3,
                0x438CCA,
                0x8493CA,
                0x8882BE,
                0xBC8DBF,
                0xF49AC2,
                0xF6989D,
                0x605CA8,
                0x855FA8,
                0xA763A8,
                0xF06EA9,
                0xF26D7D);
        }

        public override void UpdateStyle(NodeContainer renderContainer, bool isPreview)
        {
            NodeControl nodeControl = renderContainer.NodeControl;
            
            if (isPreview)
            {
                UpdateStyle(nodeControl, NodePreviewStyle);
            }
            else
            {
                if (nodeControl.AssociatedNode is RootNode)
                {
                    UpdateStyle(nodeControl, RootStyle);
                }
                else if (nodeControl.AssociatedNode.Parent is RootNode)
                {
                    UpdateStyle(nodeControl, MainStyle);
                }
                else
                {
                    UpdateStyle(nodeControl, NodeStyle);
                }

                nodeControl.ThemeColor = Colors[nodeControl.AssociatedNode.Color];
            }
        }

        private static void UpdateStyle(NodeControl nodeControl, Style style)
        {
            if (!object.ReferenceEquals(nodeControl.Style, style))
            {
                nodeControl.Style = style;
            }
        }

        public override IPathHolder CreatePreviewPath()
        {
            return new DefaultThemePath(PathPreviewStyle);
        }

        public override IPathHolder CreatePath()
        {
            return new DefaultThemePath(PathStyle);
        }

        public override void RenderPath(IPathHolder path, NodeContainer renderContainer)
        {
            ((DefaultThemePath)path).Render(renderContainer);
        }
    }
}
