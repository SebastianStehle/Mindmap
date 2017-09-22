// ==========================================================================
// IReducer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model2
{
    public interface IReducer<T>
    {
        T Reduce(T state, IAction action);
    }
}
