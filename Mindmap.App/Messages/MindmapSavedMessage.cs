// ==========================================================================
// MindmapSavedMessage.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using System;

namespace Mindmap.Messages
{
    public sealed class MindmapSavedMessage : GenericMessage<Guid>
    {
        public MindmapSavedMessage(Guid content)
            : base(content)
        {
        }
    }
}
