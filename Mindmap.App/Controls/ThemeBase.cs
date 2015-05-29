// ==========================================================================
// RendererBase.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GreenParrot.Windows.UI;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MindmapApp.Controls
{
    public class ThemeBase : FrameworkElement
    {
        private readonly List<ThemeColor> colors = new List<ThemeColor>();

        public IReadOnlyList<ThemeColor> Colors
        {
            get
            {
                return colors;
            }
        }

        protected void AddColors(params int[] newColors)
        {
            foreach (int color in newColors)
            {
                this.colors.Add(new ThemeColor(
                    new SolidColorBrush(
                        ColorsHelper.ConvertToColor((int)color, 0, 0, 0)),
                    new SolidColorBrush(
                        ColorsHelper.ConvertToColor((int)color, 0, 0.2, -0.3)),
                    new SolidColorBrush(
                        ColorsHelper.ConvertToColor((int)color, 0, -0.2, 0.2))));
            }
        }

        protected void AddColors(params ThemeColor[] newColors)
        {
            this.colors.AddRange(newColors);
        }

        public virtual void UpdateStyle(NodeContainer renderContainer, bool isPreview)
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
