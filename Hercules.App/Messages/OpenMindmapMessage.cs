// ==========================================================================
// OpenMindmapMessage.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Storage;
using GalaSoft.MvvmLight.Messaging;

namespace Hercules.App.Messages
{
    public sealed class OpenMindmapMessage : MessageBase
    {
        private readonly StorageFile file;

        public StorageFile File
        {
            get { return file; }
        }

        public OpenMindmapMessage(StorageFile file)
        {
            this.file = file;
        }
    }
}
