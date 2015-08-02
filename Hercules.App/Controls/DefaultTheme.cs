// ==========================================================================
// DefaultRenderer.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;
using System;
using Windows.UI.Xaml;

namespace Hercules.App.Controls
{
    public sealed class DefaultTheme : ThemeBase
    {
        private readonly ResourceDictionary resources = new ResourceDictionary();
        private Style pathStyle;
        private Style pathPreviewStyle;
        private Style nodePreviewStyle;
        private Style nodeLevel0Style;
        private Style nodeLevel1Style;
        private Style nodeLevel2Style;

        private Style PathStyle
        {
            get
            {
                return pathStyle ?? (pathStyle = (Style)resources["PathStyle"]);
            }
        }

        private Style PathPreviewStyle
        {
            get
            {
                return pathPreviewStyle ?? (pathPreviewStyle = (Style)resources["PathPreviewStyle"]);
            }
        }

        private Style NodePreviewStyle
        {
            get
            {
                return nodePreviewStyle ?? (nodePreviewStyle = (Style)resources["NodePreviewStyle"]);
            }
        }

        private Style NodeLevel0Style
        {
            get
            {
                return nodeLevel0Style ?? (nodeLevel0Style = (Style)resources["NodeLevel0Style"]);
            }
        }

        private Style NodeLevel1Style
        {
            get
            {
                return nodeLevel1Style ?? (nodeLevel1Style = (Style)resources["NodeLevel1Style"]);
            }
        }

        private Style NodeLevel2Style
        {
            get
            {
                return nodeLevel2Style ?? (nodeLevel2Style = (Style)resources["NodeLevel2Style"]);
            }
        }

        public DefaultTheme()
        {
            Application.LoadComponent(resources, new Uri("ms-appx:///Themes/Theme.Default.xaml"));

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
                UpdateStyle(nodeControl, (Style)resources["NodePreviewStyle"]);
            }
            else
            {
                if (nodeControl.AssociatedNode is RootNode)
                {
                    UpdateStyle(nodeControl, NodeLevel0Style);
                }
                else if (nodeControl.AssociatedNode.Parent is RootNode)
                {
                    UpdateStyle(nodeControl, NodeLevel1Style);
                }
                else
                {
                    UpdateStyle(nodeControl, NodeLevel2Style);
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
