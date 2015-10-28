// ==========================================================================
// LoadingManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GP.Windows;
using PropertyChanged;

namespace Hercules.App.Components.Implementations
{
    [ImplementPropertyChanged]
    public sealed class LoadingManager : ViewModelBase, ILoadingManager
    {
        private readonly DispatcherTimer lazyTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };

        [NotifyUI]
        public bool IsLoading { get; set; }

        public LoadingManager()
        {
            lazyTimer.Tick += LazyTimer_Tick;
        }

        private void LazyTimer_Tick(object sender, object e)
        {
            lazyTimer.Stop();

            IsLoading = false;
        }

        public void BeginLoading()
        {
            IsLoading = true;
        }

        public void FinishLoading()
        {
            lazyTimer.Start();
        }

        public async Task DoWhenNotLoadingAsync(Func<Task> action)
        {
            if (!IsLoading)
            {
                BeginLoading();
                try
                {
                    await action();
                }
                finally
                {
                    FinishLoading();
                }
            }
        }
    }
}
