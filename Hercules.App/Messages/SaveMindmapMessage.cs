// ==========================================================================
// SaveMindmapMessage.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GalaSoft.MvvmLight.Messaging;

namespace Hercules.App.Messages
{
    public sealed class SaveMindmapMessage : MessageBase
    {
        private readonly Action callback;

        public Action Callback
        {
            get { return callback; }
        }

        public SaveMindmapMessage(Action callback)
        {
            this.callback = callback;
        }
    }
}
