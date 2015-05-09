// ==========================================================================
// SaveMindmapMessage.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using System;

namespace RavenMind.Messages
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
