// ==========================================================================
// MindmapSavedMessage.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using GalaSoft.MvvmLight.Messaging;
using System;

namespace Hercules.App.Messages
{
    public sealed class MindmapSavedMessage : GenericMessage<Guid>
    {
        public MindmapSavedMessage(Guid content)
            : base(content)
        {
        }
    }
}
