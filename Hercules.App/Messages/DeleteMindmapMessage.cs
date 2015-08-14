// ==========================================================================
// DeleteMindmapMessage.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using GalaSoft.MvvmLight.Messaging;
using System;

namespace Hercules.App.Messages
{
    public sealed class DeleteMindmapMessage : GenericMessage<Guid>
    {
        public DeleteMindmapMessage(Guid id)
            : base(id)
        {
        }
    }
}
