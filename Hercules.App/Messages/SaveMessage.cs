// ==========================================================================
// SaveMessage.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GalaSoft.MvvmLight.Messaging;

namespace Hercules.App.Messages
{
    public sealed class SaveMessage : MessageBase
    {
        private readonly Action callback;

        public Action Callback
        {
            get { return callback; }
        }

        public SaveMessage(Action callback)
        {
            this.callback = callback;
        }
    }
}
