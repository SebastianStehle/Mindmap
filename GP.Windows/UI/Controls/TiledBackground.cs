// ==========================================================================
// TiledBackground.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace GP.Windows.UI.Controls
{
    /// <summary>
    /// Custom control to render a tiled background as simple grid.
    /// </summary>
    [TemplatePart(Name = PathPart, Type = typeof(Path))]
    public sealed class TiledBackground : Control
    {
        /// <summary>
        /// Identifies the name of the path template part that is used for rendering the tiles.
        /// </summary>
        public const string PathPart = "PART_Path";

        private Path path;

        /// <summary>
        /// Defines the <see cref="TileSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TileSizeProperty =
            DependencyProperty.Register("TileSize", typeof(double), typeof(TiledBackground), new PropertyMetadata(60d, OnTileSizeChanged));
        /// <summary>
        /// Gets or sets the size of the tiles.
        /// </summary>
        /// <value>The size of the tiles.</value>
        public double TileSize
        {
            get
            {
                return (double)GetValue(TileSizeProperty);
            }
            set
            {
                SetValue(TileSizeProperty, value);
            }
        }

        private static void OnTileSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as TiledBackground;
            if (owner != null)
            {
                owner.OnTileSizeChanged();
            }
        }

        private void OnTileSizeChanged()
        {
            Render();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledBackground"/> class.
        /// </summary>
        public TiledBackground()
        {
            DefaultStyleKey = typeof(TiledBackground);

            SizeChanged += TiledBackground_SizeChanged;
        }

        private void TiledBackground_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            if (path != null)
            {
                PathGeometry pathGeometry = new PathGeometry();

                double w = ActualWidth;
                double h = ActualHeight;

                for (double x = (TileSize / 2) + 0.75; x < w; x += TileSize)
                {
                    LineSegment lineSegment = new LineSegment { Point = new Point(x, h) };

                    pathGeometry.Figures.Add(new PathFigure { StartPoint = new Point(x, 0), Segments = { lineSegment } });
                }

                for (double y = (TileSize / 2) + 0.75; y < h; y += TileSize)
                {
                    LineSegment lineSegment = new LineSegment { Point = new Point(w, y) };

                    pathGeometry.Figures.Add(new PathFigure { StartPoint = new Point(0, y), Segments = { lineSegment } });
                }

                path.Data = pathGeometry;
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. 
        /// In simplest terms, this means the method  is called just before a UI element displays in your app. 
        /// Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            path = GetTemplateChild(PathPart) as Path;

            Render();
        }
    }
}
