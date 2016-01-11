// ==========================================================================
// TitleBarUpdater.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.ViewManagement;
using GP.Utils;

namespace Hercules.App.Components
{
    public sealed class TitleBarUpdater
    {
        private readonly IMindmapStore store;

        public TitleBarUpdater(IMindmapStore store)
        {
            Guard.NotNull(store, nameof(store));

            this.store = store;

            store.DocumentLoaded += Store_DocumentLoaded;
            store.MindmapUpdated += Store_MindmapUpdated;
        }

        private void Store_MindmapUpdated(object sender, EventArgs e)
        {
            UpdateTitle();
        }

        private void Store_DocumentLoaded(object sender, DocumentLoadedEventArgs e)
        {
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            ApplicationView.GetForCurrentView().Title = store.LoadedMindmap != null ? store.LoadedMindmap.Name : string.Empty;
        }
    }
}
