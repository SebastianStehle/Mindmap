// ==========================================================================
// TitleBarUpdater.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using GP.Utils;
using Hercules.Model.Storing;

namespace Hercules.App.Components
{
    public sealed class TitleBarUpdater
    {
        private readonly IMindmapStore store;
        private DocumentFile currentFile;

        public TitleBarUpdater(IMindmapStore store)
        {
            Guard.NotNull(store, nameof(store));

            this.store = store;

            store.DocumentLoaded += Store_DocumentLoaded;
        }

        private void Store_DocumentLoaded(object sender, DocumentFileEventArgs e)
        {
            UpdateTitle(e.File);

            if (currentFile != null)
            {
                currentFile.PropertyChanged -= CurrentFile_PropertyChanged;
            }

            currentFile = e.File;

            if (currentFile != null)
            {
                currentFile.PropertyChanged += CurrentFile_PropertyChanged;
            }
        }

        private void CurrentFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTitle(currentFile);
        }

        private static void UpdateTitle(DocumentFile file)
        {
            string name = string.Empty;

            if (file != null)
            {
                name = file.Name;

                if (file.HasChanges)
                {
                    name += "*";
                }
            }

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    ApplicationView view = ApplicationView.GetForCurrentView();

                    if (view != null)
                    {
                        view.Title = name;
                    }
                }).Forget();
        }
    }
}
