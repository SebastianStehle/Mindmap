// ==========================================================================
// MindmapSavedMessage.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using System;

namespace RavenMind.Messages
{
    public sealed class MindmapSavedMessage : GenericMessage<Guid>
    {
        public MindmapSavedMessage(Guid content)
            : base(content)
        {
        }
    }
}
