// ==========================================================================
// DocumentFileRecentList.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Hercules.Model.Storing
{
    public sealed class DocumentFileRecentList
    {
        private readonly StorageItemMostRecentlyUsedList recentList = StorageApplicationPermissions.MostRecentlyUsedList;
        private readonly Dictionary<string, string> tokenMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<DocumentFile> files = new List<DocumentFile>();

        public IReadOnlyList<DocumentFile> Files
        {
            get { return files; }
        }

        public Task LoadAsync()
        {
            return FileQueue.EnqueueAsync(async () =>
            {
                tokenMapping.Clear();

                List<DocumentFile> unsortedFiles = new List<DocumentFile>();

                foreach (AccessListEntry entry in recentList.Entries.ToList())
                {
                    try
                    {
                        StorageFile file = await recentList.GetFileAsync(entry.Token);

                        tokenMapping[file.Path] = entry.Token;

                        unsortedFiles.Add(await DocumentFile.OpenAsync(file));
                    }
                    catch (FileNotFoundException)
                    {
                        recentList.Remove(entry.Token);
                    }
                }

                files.Clear();

                foreach (DocumentFile sortedFile in unsortedFiles.OrderByDescending(x => x.ModifiedUtc))
                {
                    files.Add(sortedFile);
                }
            });
        }

        public void Add(DocumentFile file)
        {
            Add(file?.File);
        }

        public void Add(StorageFile storageFile)
        {
            if (storageFile?.Path != null)
            {
                string token = recentList.Add(storageFile);

                tokenMapping[storageFile.Path] = token;
            }
        }

        public void Remove(DocumentFile file)
        {
            StorageFile storageFile = file?.File;

            string token;

            if (storageFile?.Path != null && tokenMapping.TryGetValue(storageFile.Path, out token))
            {
                recentList.Remove(token);

                tokenMapping.Remove(storageFile.Path);
            }
        }
    }
}
