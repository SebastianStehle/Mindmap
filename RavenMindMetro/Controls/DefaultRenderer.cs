using RavenMind.Model;
using Windows.UI.Xaml;

namespace RavenMind.Controls
{
    public sealed class DefaultRenderer : RendererBase
    {
        public static readonly DependencyProperty PathStyleProperty =
            DependencyProperty.Register("PathStyle", typeof(Style), typeof(DefaultRenderer), new PropertyMetadata(null));
        public Style PathStyle
        {
            get { return (Style)GetValue(PathStyleProperty); }
            set { SetValue(PathStyleProperty, value); }
        }

        public static readonly DependencyProperty PathPreviewStyleProperty =
            DependencyProperty.Register("PathPreviewStyle", typeof(Style), typeof(DefaultRenderer), new PropertyMetadata(null));
        public Style PathPreviewStyle
        {
            get { return (Style)GetValue(PathPreviewStyleProperty); }
            set { SetValue(PathPreviewStyleProperty, value); }
        }

        public static readonly DependencyProperty RootStyleProperty =
            DependencyProperty.Register("RootStyle", typeof(Style), typeof(DefaultRenderer), new PropertyMetadata(null));
        public Style RootStyle
        {
            get { return (Style)GetValue(RootStyleProperty); }
            set { SetValue(RootStyleProperty, value); }
        }

        public static readonly DependencyProperty NodePreviewStyleProperty =
            DependencyProperty.Register("NodePreviewStyle", typeof(Style), typeof(DefaultRenderer), new PropertyMetadata(null));
        public Style NodePreviewStyle
        {
            get { return (Style)GetValue(NodePreviewStyleProperty); }
            set { SetValue(NodePreviewStyleProperty, value); }
        }

        public static readonly DependencyProperty MainStyleProperty =
            DependencyProperty.Register("MainStyle", typeof(Style), typeof(DefaultRenderer), new PropertyMetadata(null));
        public Style MainStyle
        {
            get { return (Style)GetValue(MainStyleProperty); }
            set { SetValue(MainStyleProperty, value); }
        }

        public static readonly DependencyProperty NodeStyleProperty =
            DependencyProperty.Register("NodeStyle", typeof(Style), typeof(DefaultRenderer), new PropertyMetadata(null));
        public Style NodeStyle
        {
            get { return (Style)GetValue(NodeStyleProperty); }
            set { SetValue(NodeStyleProperty, value); }
        }

        public DefaultRenderer()
        {
        }

        public override Style CalculateStyle(NodeContainer renderContainer, bool isPreview)
        {
            if (isPreview)
            {
                return NodePreviewStyle;
            }
            else if (renderContainer.NodeControl.AssociatedNode is RootNode)
            {
                return RootStyle;
            }
            else if (renderContainer.NodeControl.AssociatedNode.Parent is RootNode)
            {
                return MainStyle;
            }
            else
            {
                return NodeStyle;
            }
        }

        public override IPathHolder CreatePreviewPath()
        {
            return new DefaultPath(PathPreviewStyle);
        }

        public override IPathHolder CreatePath()
        {
            return new DefaultPath(PathStyle);
        }

        public override void RenderPath(IPathHolder path, NodeContainer renderContainer)
        {
            ((DefaultPath)path).Render(renderContainer);
        }
    }
}
