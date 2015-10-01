// ==========================================================================
// ILoadingManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.App.Components
{
    public interface ILoadingManager
    {
        bool IsLoading { get; set; }

        void BeginLoading();

        void FinishLoading();
    }
}
