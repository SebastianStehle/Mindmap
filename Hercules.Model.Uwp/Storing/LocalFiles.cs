// ==========================================================================
// LocalFiles.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using GP.Utils;

namespace Hercules.Model.Storing
{
    public static class LocalFiles
    {
        public static Task<bool> CopyToDocumentsAsync(DocumentFileRecentList recentList)
        {
            Guard.NotNull(recentList, nameof(recentList));

            return FileQueue.EnqueueAsync(async () =>
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFolder mindmaps = null;
                try
                {
                    mindmaps = await localFolder.GetFolderAsync("Mindapp");
                }
                catch (FileNotFoundException)
                {
                    mindmaps = null;
                }

                try
                {
                    if (mindmaps != null)
                    {
                        IReadOnlyList<StorageFile> files = await mindmaps.GetFilesAsync();

                        StorageFolder documents = KnownFolders.DocumentsLibrary;

                        foreach (StorageFile file in files)
                        {
                            StorageFile copy = await file.CopyAsync(documents, file.Name, NameCollisionOption.GenerateUniqueName);

                            recentList.Add(copy);
                        }
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}
