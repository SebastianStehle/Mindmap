// ==========================================================================
// OpenMindmapMessage.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using System;

namespace Mindmap.Messages
{
    public sealed class OpenMindmapMessage : GenericMessage<Guid?>
    {
        public OpenMindmapMessage()
            : base(null)
        {
        }

        public OpenMindmapMessage(Guid id)
            : base(id)
        {
        }
    }
}
