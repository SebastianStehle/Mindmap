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
    [TemplatePart(Name = PanelPart, Type = typeof(Border))]
    public sealed class ColorPresenter : Control
    {
        private const string PanelPart = "PART_Panel";
        private Border border;

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(INodeColor), typeof(ColorPresenter), new PropertyMetadata(null, (d, e) => ((ColorPresenter)d).UpdateColor()));
        public INodeColor Color
        {
            get { return (INodeColor)GetValue(ColorProperty); }
            set { SetValue(RendererProperty, value); }
        }

        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.Register(nameof(Renderer), typeof(Win2DRenderer), typeof(ColorPresenter), new PropertyMetadata(null, (d, e) => ((ColorPresenter)d).UpdateColor()));
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
            border = GetTemplateChild(PanelPart) as Border;

            UpdateColor();
        }

        private void UpdateColor()
        {
            if (border == null || Color == null)
            {
                return;
            }

            ThemeColor themeColor = Color as ThemeColor;

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
