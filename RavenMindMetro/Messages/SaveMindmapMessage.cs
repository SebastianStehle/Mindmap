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
        #region Fields

        private readonly Action callback;

        #endregion

        #region Constructors

        public SaveMindmapMessage(Action messageCallback)
        {
            callback = messageCallback;
        }

        #endregion

        #region Methods

        public void Complete()
        {
            if (callback != null)
            {
                callback();
            }
        }

        #endregion
    }
}
