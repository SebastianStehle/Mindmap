// ==========================================================================
// DocumentLoadedEventArgs.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Hercules.Model;

namespace Hercules.App.Components
{
    public sealed class DocumentLoadedEventArgs : EventArgs
    {
        private readonly Document document;

        public Document Document
        {
            get { return document; }
        }

        public DocumentLoadedEventArgs(Document document)
        {
            this.document = document;
        }
    }
}
