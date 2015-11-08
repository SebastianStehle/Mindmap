﻿// ==========================================================================
// IWin2DRendererProvider.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows.UI.Controls;
using Hercules.Model;

namespace Hercules.Win2D.Rendering
{
    public interface IWin2DRendererProvider
    {
        Win2DRenderer Current { get; }

        Win2DRenderer CreateRenderer(Document document, ICanvasControl canvas);
    }
}