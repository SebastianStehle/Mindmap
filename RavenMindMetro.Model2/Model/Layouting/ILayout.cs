// ==========================================================================
// INodeView.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;

namespace RavenMind.Model.Layouting
{
    public interface ILayout
    {
        double HorizontalMargin { get; }

        double ElementMargin { get; }

        void UpdateLayout(Document document, IRen  Size availableSize);
    }
}
