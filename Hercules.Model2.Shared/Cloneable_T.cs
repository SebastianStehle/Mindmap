// ==========================================================================
// Cloneable_T.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model2
{
    public abstract class Cloneable<T> : Cloneable where T : Cloneable
    {
        protected T Clone(Action<T> change)
        {
            var result = (T)MemberwiseClone();

            change(result);

            result.OnCloned();

            return result;
        }
    }
}
