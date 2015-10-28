// ==========================================================================
// ILoadingManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;

namespace Hercules.App.Components
{
    public interface ILoadingManager
    {
        bool IsLoading { get; set; }

        void BeginLoading();

        void FinishLoading();

        Task DoWhenNotLoadingAsync(Func<Task> action);
    }
}
