// ==========================================================================
// OpenMessage.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Storage;
using GalaSoft.MvvmLight.Messaging;

namespace Hercules.App.Messages
{
    public sealed class OpenMessage : MessageBase
    {
        private readonly StorageFile file;

        public StorageFile File
        {
            get { return file; }
        }

        public OpenMessage(StorageFile file)
        {
            this.file = file;
        }
    }
}
