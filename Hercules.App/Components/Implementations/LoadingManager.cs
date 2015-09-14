// ==========================================================================
// LoadingManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight;
using GP.Windows;
using PropertyChanged;

namespace Hercules.App.Components.Implementations
{
    [ImplementPropertyChanged]
    public sealed class LoadingManager : ViewModelBase, ILoadingManager
    {
        [NotifyUI]
        public bool IsLoading { get; set; }

        public void BeginLoading()
        {
            IsLoading = true;
        }

        public void FinishLoading()
        {
            IsLoading = false;
        }
    }
}
