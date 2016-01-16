// ==========================================================================
// DocumentLoadedEventArgs.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.App.Components
{
    public sealed class DocumentFileEventArgs : EventArgs
    {
        private readonly DocumentFileModel file;

        public DocumentFileModel File
        {
            get { return file; }
        }

        public DocumentFileEventArgs(DocumentFileModel file)
        {
            this.file = file;
        }
    }
}
