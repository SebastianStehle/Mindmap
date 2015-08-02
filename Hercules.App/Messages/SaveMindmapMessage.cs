// ==========================================================================
// SaveMindmapMessage.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using System;

namespace Hercules.App.Messages
{
    public sealed class SaveMindmapMessage : MessageBase
    {
        private readonly Action callback;
        
        public SaveMindmapMessage(Action messageCallback)
        {
            callback = messageCallback;
        }

        public void Complete()
        {
            if (callback != null)
            {
                callback();
            }
        }
    }
}
