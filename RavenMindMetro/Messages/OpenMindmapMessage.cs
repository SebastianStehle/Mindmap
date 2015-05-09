// ==========================================================================
// OpenMindmapMessage.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;
using System;

namespace RavenMind.Messages
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
