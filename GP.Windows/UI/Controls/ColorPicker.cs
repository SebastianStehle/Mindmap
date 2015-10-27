// ==========================================================================
// ColorPicker.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace GP.Windows.UI.Controls
{
    /// <summary>
    /// Implements a color picker control.
    /// </summary>
    [TemplatePart(Name = ColorThumbPart, Type = typeof(Thumb))]
    [TemplatePart(Name = ValueBackgroundPart, Type = typeof(Rectangle))]
    [TemplatePart(Name = HueBackgroundPart, Type = typeof(Rectangle))]
    [TemplatePart(Name = ValueSliderPart, Type = typeof(Slider))]
    public sealed class ColorPicker : Control
    {
        private const string ColorThumbPart = "ColorThumb";
        private const string ValueBackgroundPart = "ValueBackground";
        private const string HueBackgroundPart = "HueBackground";
        private const string ValueSliderPart = "ValueSlider";
        private readonly TranslateTransform translateTransform = new TranslateTransform();
        private Rectangle hueBackground;
        private Slider valueSlider;
        private Thumb colorThumb;
        private bool isUpdating;
        private bool isTemplateApplied;

        /// <summary>
        /// Raised when the selected color has changed.
        /// </summary>
        public event EventHandler SelectedColorChanged;

        /// <summary>
        /// Defines the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Red, (d, e) => ((ColorPicker)d).OnSelectedColorChanged()));
        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="HueProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0, (d, e) => ((ColorPicker)d).OnHueChanged()));
        /// <summary>
        /// Gets or sets the hue from the selected color (HSV color space).
        /// </summary>
        public int Hue
        {
            get { return (int)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="Saturation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(int), typeof(ColorPicker), new PropertyMetadata(100, (d, e) => ((ColorPicker)d).OnSaturationChanged()));
        /// <summary>
        /// Gets or sets the saturation from the selected color (HSV color space).
        /// </summary>
        public int Saturation
        {
            get { return (int)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(ColorPicker), new PropertyMetadata(100, (d, e) => ((ColorPicker)d).OnValueChanged()));
        /// <summary>
        /// Gets or sets the value from the selected color (HSV color space).
        /// </summary>
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private void OnSelectedColorChanged()
        {
            if (isTemplateApplied)
            {
                SelectedColorChanged?.Invoke(this, EventArgs.Empty);
            }

            MakeNonRecursive(() =>
            {
                double h, s, v;

                ColorsHelper.ColorToHSV(SelectedColor, out h, out s, out v);

                UpdateHue(h);
                UpdateValue(v);
                UpdateSaturation(s);

                RepaintColorThumb();
                RepaintHueBackground();
                RepaintSlider();
            });
        }

        private void OnValueChanged()
        {
            if (CoerceValue(ValueProperty, 100))
            {
                MakeNonRecursive(() =>
                {
                    MoveThumbByValue();

                    UpdateSelectedColor();

                    RepaintColorThumb();
                });
            }
        }

        private void OnSaturationChanged()
        {
            if (CoerceValue(SaturationProperty, 100))
            {
                MakeNonRecursive(() =>
                {
                    MoveThumbBySaturation();

                    UpdateSelectedColor();

                    RepaintColorThumb();
                });
            }
        }

        private void OnHueChanged()
        {
            if (CoerceValue(HueProperty, 360))
            {
                MakeNonRecursive(() =>
                {
                    UpdateSelectedColor();

                    RepaintHueBackground();
                    RepaintSlider();
                    RepaintColorThumb();
                });
            }
        }

        private bool CoerceValue(DependencyProperty property, int maxValue)
        {
            int current = (int)GetValue(property);
            int coerced = Math.Max(0, Math.Min(current, maxValue));

            bool isOkay = current == coerced;

            if (!isOkay)
            {
                SetValue(property, coerced);
            }

            return isOkay;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPicker"/> class.
        /// </summary>
        public ColorPicker()
        {
            DefaultStyleKey = typeof(ColorPicker);

            SizeChanged += ColorPicker_SizeChanged;
        }

        private void ColorPicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MoveThumbBySaturation();
            MoveThumbByValue();
        }

        /// <summary>
        /// Binds the controls when the template is applied for the color picker.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            BindColorThumb();
            BindHueBackground();
            BindValueBackground();
            BindValueSlider();

            MoveThumbBySaturation();
            MoveThumbByValue();

            RepaintColorThumb();
            RepaintSlider();
            RepaintHueBackground();

            isTemplateApplied = true;
        }

        private void BindValueSlider()
        {
            valueSlider = GetTemplateChild(ValueSliderPart) as Slider;
        }

        private void BindColorThumb()
        {
            colorThumb = GetTemplateChild(ColorThumbPart) as Thumb;

            if (colorThumb != null)
            {
                colorThumb.RenderTransform = translateTransform;
            }
        }

        private void BindValueBackground()
        {
            Rectangle valueBackground = GetTemplateChild(ValueBackgroundPart) as Rectangle;

            if (valueBackground != null)
            {
                valueBackground.Fill = ColorsHelper.GetColorGradientBrush(Orientation.Horizontal);
            }
        }

        private void BindHueBackground()
        {
            hueBackground = GetTemplateChild(HueBackgroundPart) as Rectangle;

            if (hueBackground != null)
            {
                hueBackground.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
                hueBackground.PointerPressed += HueBackground_PointerPressed;
                hueBackground.ManipulationDelta += HueBackground_ManipulationDelta;
                hueBackground.ManipulationStarted += HueBackground_ManipulationStarted;
            }
        }

        private void HueBackground_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Point position = e.GetCurrentPoint(hueBackground).Position;

            translateTransform.X = position.X;
            translateTransform.Y = position.Y;

            UpdateByTranslation();
        }

        private void HueBackground_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            Point position = e.Position;

            translateTransform.X = position.X;
            translateTransform.Y = position.Y;

            UpdateByTranslation();
        }

        private void HueBackground_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            translateTransform.X += e.Delta.Translation.X;
            translateTransform.Y += e.Delta.Translation.Y;

            UpdateByTranslation();
        }

        private void MakeNonRecursive(Action action)
        {
            if (!isUpdating)
            {
                isUpdating = true;
                try
                {
                    action();
                }
                finally
                {
                    isUpdating = false;
                }
            }
        }

        private void UpdateByTranslation()
        {
            if (translateTransform.X < 0)
            {
                translateTransform.X = 0;
            }

            if (translateTransform.Y < 0)
            {
                translateTransform.Y = 0;
            }

            if (hueBackground != null)
            {
                if (translateTransform.X > hueBackground.ActualWidth)
                {
                    translateTransform.X = hueBackground.ActualWidth;
                }

                if (translateTransform.Y > hueBackground.ActualHeight)
                {
                    translateTransform.Y = hueBackground.ActualHeight;
                }
            }

            MakeNonRecursive(() =>
            {
                UpdateSaturationByTransform();
                UpdateValueByTransform();
                UpdateSelectedColor();
                RepaintColorThumb();
            });
        }

        private void UpdateSelectedColor()
        {
            SelectedColor = ColorsHelper.ColorFromHSV(Hue, Saturation / 100f, Value / 100f);
        }

        private void MoveThumbByValue()
        {
            if (hueBackground != null)
            {
                translateTransform.Y = Math.Round(((100 - Value) / 100f) * hueBackground.ActualHeight);
            }
        }

        private void UpdateValueByTransform()
        {
            if (hueBackground != null)
            {
                Value = 100 - (int)Math.Round(((translateTransform.Y / hueBackground.ActualHeight) * 100));
            }
        }

        private void MoveThumbBySaturation()
        {
            if (hueBackground != null)
            {
                translateTransform.X = Math.Round((Saturation / 100f) * hueBackground.ActualWidth);
            }
        }

        private void UpdateSaturationByTransform()
        {
            if (hueBackground != null)
            {
                Saturation = (int)Math.Round((translateTransform.X / hueBackground.ActualWidth) * 100);
            }
        }

        private void UpdateSaturation(double saturation)
        {
            Saturation = (int)Math.Round((saturation * 100));
        }

        private void UpdateValue(double value)
        {
            Value = (int)Math.Round((value * 100));
        }

        private void UpdateHue(double hue)
        {
            Hue = (int)hue;
        }

        private void RepaintColorThumb()
        {
            if (colorThumb != null)
            {
                colorThumb.Foreground = new SolidColorBrush(SelectedColor);
            }
        }

        private void RepaintSlider()
        {
            if (valueSlider != null)
            {
                valueSlider.Foreground = new SolidColorBrush(ColorsHelper.ColorFromHSV(Hue, 1, 1));
            }
        }

        private void RepaintHueBackground()
        {
            if (hueBackground != null)
            {
                hueBackground.Fill = new SolidColorBrush(ColorsHelper.ColorFromHSV(Hue, 1, 1));
            }
        }
    }
}
