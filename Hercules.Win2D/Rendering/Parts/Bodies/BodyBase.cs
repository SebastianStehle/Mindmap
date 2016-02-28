﻿// ==========================================================================
// BodyBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.Foundation;
using Hercules.Model;
using Hercules.Model.Rendering;
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Hercules.Win2D.Rendering.Parts.Bodies
{
    public abstract class BodyBase : IBodyPart
    {
        protected const float ButtonRadius = 10;
        protected const float CheckBoxSize = 20;
        protected const float CheckBoxMargin = 6;
        protected const float MinHeight = 36;
        protected const float IconSizeLarge = 160;
        protected const float IconSizeMedium = 64;
        protected const float IconSizeSmall = 32;
        protected const float IconMargin = 6;
        protected static readonly CanvasStrokeStyle SelectionStrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };
        private readonly Win2DTextRenderer textRenderer = new Win2DTextRenderer();
        private readonly ExpandButton button = new ExpandButton();
        private readonly CheckBox checkBox = new CheckBox();
        private Vector2 textIconRenderSize;
        private Vector2 textIconPadding;
        private Vector2 iconRenderSize;
        private Vector2 iconRenderPosition;

        public virtual Win2DTextRenderer TextRenderer
        {
            get { return textRenderer; }
        }

        public virtual bool HasText
        {
            get { return true; }
        }

        public virtual float VerticalPathOffset
        {
            get { return 0; }
        }

        public virtual void ClearResources()
        {
            textRenderer.ClearResources();
        }

        public bool HandleClick(Win2DRenderable renderable, Vector2 hitPosition)
        {
            return button.HandleClick(renderable, hitPosition) || checkBox.HandleClick(renderable, hitPosition);
        }

        public virtual Vector2 Measure(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator)
        {
            textRenderer.Measure(renderable, resourceCreator);

            textIconRenderSize = textRenderer.RenderSize;

            if (renderable.Node.Icon != null)
            {
                float targetSize = IconSizeSmall;

                if (renderable.Node.IconSize == IconSize.Medium)
                {
                    targetSize = IconSizeMedium;
                }
                else if (renderable.Node.IconSize == IconSize.Large)
                {
                    targetSize = IconSizeLarge;
                }

                iconRenderSize = new Vector2(renderable.Node.Icon.PixelWidth, renderable.Node.Icon.PixelHeight);

                float ratio = iconRenderSize.X / iconRenderSize.Y;

                if (iconRenderSize.X > iconRenderSize.Y)
                {
                    iconRenderSize = new Vector2(targetSize, targetSize / ratio);
                }
                else
                {
                    iconRenderSize = new Vector2(targetSize * ratio, targetSize);
                }

                if (renderable.Node.IconPosition == IconPosition.Left || renderable.Node.IconPosition == IconPosition.Right)
                {
                    textIconRenderSize.X += iconRenderSize.X;
                    textIconRenderSize.X += IconMargin;

                    textIconRenderSize.Y = Math.Max(textIconRenderSize.Y, iconRenderSize.Y);
                }
                else
                {
                    textIconRenderSize.Y += iconRenderSize.Y;
                    textIconRenderSize.Y += IconMargin;

                    textIconRenderSize.X = Math.Max(textIconRenderSize.X, iconRenderSize.X);
                }
            }

            textIconRenderSize.Y = Math.Max(MinHeight, textIconRenderSize.Y);
            textIconPadding = CalculatePadding(renderable, textIconRenderSize);

            Vector2 totalSize = textIconRenderSize + (2 * textIconPadding);

            if (MustRenderCheckBox(renderable))
            {
                totalSize.X += CheckBoxSize + CheckBoxMargin;
            }

            return totalSize;
        }

        public virtual void Arrange(Win2DRenderable renderable, ICanvasResourceCreator resourceCreator)
        {
            Vector2 iconOffset = Vector2.Zero;
            Vector2 textOffset = Vector2.Zero;

            Vector2 textRenderSize = textRenderer.RenderSize;

            if (renderable.Node.Icon == null)
            {
                textOffset.Y = 0.5f * (textIconRenderSize.Y - textRenderSize.Y);
            }
            else if (renderable.Node.IconPosition == IconPosition.Left)
            {
                textOffset.Y = 0.5f * (textIconRenderSize.Y - textRenderSize.Y);
                textOffset.X = iconRenderSize.X + IconMargin;

                iconOffset.Y = 0.5f * (textIconRenderSize.Y - iconRenderSize.Y);
            }
            else if (renderable.Node.IconPosition == IconPosition.Right)
            {
                textOffset.Y = 0.5f * (textIconRenderSize.Y - textRenderSize.Y);

                iconOffset.X = textRenderSize.X + IconMargin;
                iconOffset.Y = 0.5f * (textIconRenderSize.Y - iconRenderSize.Y);
            }
            else if (renderable.Node.IconPosition == IconPosition.Top)
            {
                textOffset.X = 0.5f * (textIconRenderSize.X - textRenderSize.X);
                textOffset.Y = iconRenderSize.Y + IconMargin;

                iconOffset.X = 0.5f * (textIconRenderSize.X - iconRenderSize.X);
            }
            else if (renderable.Node.IconPosition == IconPosition.Bottom)
            {
                textOffset.X = 0.5f * (textIconRenderSize.X - textRenderSize.X);

                iconOffset.X = 0.5f * (textIconRenderSize.X - iconRenderSize.X);
                iconOffset.Y = textRenderSize.Y + IconMargin;
            }

            if (MustRenderButton(renderable))
            {
                ArrangeButton(renderable);
            }

            if (MustRenderCheckBox(renderable))
            {
                Vector2 checkBoxOffset = new Vector2(
                    textIconPadding.X + (0.5f * CheckBoxSize),
                    textIconPadding.Y + textOffset.Y + (0.5f * textRenderSize.Y));

                checkBox.Arrange(renderable.RenderPosition + checkBoxOffset, CheckBoxSize);

                textOffset.X += CheckBoxSize + CheckBoxMargin;
                iconOffset.X += CheckBoxSize + CheckBoxMargin;
            }

            iconRenderPosition = renderable.RenderPosition + textIconPadding + iconOffset;

            Vector2 textRenderPosition = renderable.RenderPosition + textIconPadding + textOffset;

            textRenderer.Arrange(textRenderPosition);
        }

        private void ArrangeButton(IRenderable renderable)
        {
            Vector2 renderPosition = renderable.RenderPosition;
            Vector2 renderSize = renderable.RenderSize;
            Vector2 buttonPosition;

            if (renderable.Node.NodeSide == NodeSide.Left)
            {
                buttonPosition = new Vector2(
                    renderPosition.X - 2,
                    renderPosition.Y + (renderSize.Y * 0.5f));
            }
            else
            {
                buttonPosition = new Vector2(
                    renderPosition.X + (renderSize.X + 2),
                    renderPosition.Y + (renderSize.Y * 0.5f));
            }

            button.Arrange(buttonPosition, ButtonRadius);
        }

        protected void RenderText(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            textRenderer.Render(renderable, session);
        }

        protected void RenderCheckBox(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            if (!MustRenderCheckBox(renderable))
            {
                return;
            }

            checkBox.Render(renderable, session);
        }

        protected void RenderButton(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            if (!MustRenderButton(renderable))
            {
                return;
            }

            button.Render(renderable, session);
        }

        private static bool MustRenderCheckBox(IRenderable renderable)
        {
             return renderable.Node.IsCheckable;
        }

        private static bool MustRenderButton(IRenderable renderable)
        {
            return renderable.Node.HasChildren;
        }

        protected void RenderIcon(Win2DRenderable renderable, CanvasDrawingSession session)
        {
            if (renderable.Node.Icon == null)
            {
                return;
            }

            ICanvasImage image = renderable.Resources.Image(renderable.Node);

            if (image == null)
            {
                return;
            }

            Rect rect = new Rect(
                iconRenderPosition.X,
                iconRenderPosition.Y,
                iconRenderSize.X,
                iconRenderSize.Y);

            session.DrawImage(image, rect, image.GetBounds(session), 1, CanvasImageInterpolation.HighQualityCubic);
        }

        protected virtual Vector2 CalculatePadding(Win2DRenderable renderable, Vector2 contentSize)
        {
            return Vector2.Zero;
        }

        public abstract void Render(Win2DRenderable renderable, CanvasDrawingSession session, Win2DColor color, bool renderControls);

        public abstract IBodyPart Clone();
    }
}