// ==========================================================================
// OpenMindmapMessage.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using Hercules.App.Modules;

namespace Hercules.App.Messages
{
    public sealed class OpenMindmapMessage : GenericMessage<MindmapItem>
    {
        public OpenMindmapMessage(MindmapItem documentRef)
            : base(documentRef)
        {
        }
    }
}
