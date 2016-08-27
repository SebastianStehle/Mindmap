// ==========================================================================
// ImmutableObject.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public abstract class ImmutableObject<TDerived> where TDerived : class
    {
        protected TDerived Cloned<TCurrent>(params Action<TCurrent>[] modifiers)
        {
            object clone = MemberwiseClone();

            foreach (var modifier in modifiers)
            {
                modifier((TCurrent)clone);
            }

            return (TDerived)clone;
        }
    }
}
