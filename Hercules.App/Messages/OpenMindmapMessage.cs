// ==========================================================================
// OpenMindmapMessage.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using Hercules.Model.Storing;

namespace Hercules.App.Messages
{
    public sealed class OpenMindmapMessage : MessageBase
    {
        private readonly DocumentFile file;

        public DocumentFile File
        {
            get { return file; }
        }

        public OpenMindmapMessage(DocumentFile file)
        {
            this.file = file;
        }
    }
}
