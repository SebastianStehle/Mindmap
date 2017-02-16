// ==========================================================================
// ColorPresenter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using GP.Utils.Mathematics;
using GP.Utils.UI;
using Hercules.Model;
using Hercules.Win2D.Rendering;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Hercules.App.Controls
{
    [TemplatePart(Name = PartPanel, Type = typeof(Border))]
    public sealed class ColorPresenter : Control
    {
        private const string PartPanel = "PART_Panel";
        private Border border;

        public static readonly DependencyProperty ColorProperty =
            DependencyPropertyManager.Register<ColorPresenter, INodeColor>(nameof(Color), null, e => e.Owner.UpdateColor());
        public INodeColor Color
        {
            get { return (INodeColor)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty RendererProperty =
            DependencyPropertyManager.Register<ColorPresenter, Win2DRenderer>(nameof(Renderer), null, e => e.Owner.UpdateColor());
        public Win2DRenderer Renderer
        {
            get { return (Win2DRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        public ColorPresenter()
        {
            DefaultStyleKey = typeof(ColorPresenter);
        }

        protected override void OnApplyTemplate()
        {
            border = GetTemplateChild(PartPanel) as Border;

            UpdateColor();
        }

        private void UpdateColor()
        {
            if (border == null || Color == null)
            {
                return;
            }

            var themeColor = Color as ThemeColor;

            Color color;

            if (themeColor != null)
            {
                color = Renderer.Resources.Colors[themeColor.Index].Normal.ToColor();
            }
            else
            {
                color = ColorsHelper.ConvertToColor(((ValueColor)Color).Color);
            }

            border.Background = new SolidColorBrush(color);
        }
    }
}
