using System;
using Windows.UI.Xaml;

namespace RavenMind.Controls
{
    public class RendererBase : FrameworkElement
    {
        public virtual Style CalculateStyle(NodeContainer renderContainer, bool isPreview)
        {
            throw new NotImplementedException();
        }

        public virtual IPathHolder CreatePreviewPath()
        {
            throw new NotImplementedException();
        }

        public virtual IPathHolder CreatePath()
        {
            throw new NotImplementedException();
        }

        public virtual void RenderPath(IPathHolder path, NodeContainer renderContainer)
        {
            throw new NotImplementedException();
        }
    }
}
