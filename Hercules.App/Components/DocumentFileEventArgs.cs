// ==========================================================================
// DocumentLoadedEventArgs.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Hercules.Model.Storing;

namespace Hercules.App.Components
{
    public sealed class DocumentFileEventArgs : EventArgs
    {
        private readonly DocumentFile file;

        public DocumentFile File
        {
            get { return file; }
        }

        public DocumentFileEventArgs(DocumentFile file)
        {
            this.file = file;
        }
    }
}
