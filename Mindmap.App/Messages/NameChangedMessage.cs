// ==========================================================================
// NameChangedMessage.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight.Messaging;

namespace Mindmap.Messages
{
    public sealed class NameChangedMessage : GenericMessage<string>
    {
        public NameChangedMessage(string content)
            : base(content)
        {
        }
    }
}
