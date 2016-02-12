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
using GP.Utils;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Model.Storing
{
    public sealed class DocumentFileRecentList
    {
        private const string DefaultSubfolder = "Mindapp";
        private readonly StorageItemMostRecentlyUsedList recentList = StorageApplicationPermissions.MostRecentlyUsedList;
        private readonly Dictionary<string, string> tokenMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<DocumentFile> files = new List<DocumentFile>();

        public async Task LoadAsync<T>(ICollection<T> list, Func<DocumentFile, T> factory)
        {
            Guard.NotNull(list, nameof(list));
            Guard.NotNull(factory, nameof(factory));

            IReadOnlyList<DocumentFile> loadedFiles = await LoadAsync();

            loadedFiles.Foreach(x => list.Add(factory(x)));
        }

        public Task<IReadOnlyList<DocumentFile>> LoadAsync()
        {
            return FileQueue.EnqueueAsync<IReadOnlyList<DocumentFile>>(async () =>
            {
                Dictionary<string, DocumentFile> unsortedFiles = new Dictionary<string, DocumentFile>(StringComparer.OrdinalIgnoreCase);

                await AddFilesLocalAsync(unsortedFiles);
                await AddFilesRecentAsync(unsortedFiles);

                files.Clear();

                foreach (DocumentFile sortedFile in unsortedFiles.Values.OrderByDescending(x => x.ModifiedUtc))
                {
                    files.Add(sortedFile);
                }

                return files;
            });
        }

        private static async Task AddFilesLocalAsync(IDictionary<string, DocumentFile> unsortedFiles)
        {
            StorageFolder mindmaps;
            try
            {
                mindmaps = await ApplicationData.Current.LocalFolder.GetFolderAsync(DefaultSubfolder);
            }
            catch (FileNotFoundException)
            {
                mindmaps = null;
            }

            if (mindmaps == null)
            {
                return;
            }

            foreach (StorageFile file in await mindmaps.GetFilesAsync())
            {
                if (!unsortedFiles.ContainsKey(file.Path))
                {
                    unsortedFiles.Add(file.Path, await DocumentFile.OpenAsync(file));
                }
            }
        }

        private async Task AddFilesRecentAsync(IDictionary<string, DocumentFile> unsortedFiles)
        {
            tokenMapping.Clear();

            HashSet<string> filesHandled = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (AccessListEntry entry in recentList.Entries.ToList())
            {
                try
                {
                    StorageFile file = await recentList.GetFileAsync(entry.Token);

                    if (!filesHandled.Add(file.Path))
                    {
                        continue;
                    }

                    tokenMapping[file.Path] = entry.Token;

                    if (unsortedFiles.ContainsKey(file.Path))
                    {
                        recentList.Remove(entry.Token);
                    }
                    else
                    {
                        unsortedFiles.Add(file.Path, await DocumentFile.OpenAsync(file));
                    }
                }
                catch (FileNotFoundException)
                {
                    recentList.Remove(entry.Token);
                }
            }
        }

        public Task SaveAsync(IEnumerable<DocumentFile> newFiles)
        {
            Guard.NotNull(newFiles, nameof(newFiles));

            return FileQueue.EnqueueAsync(() =>
            {
                HashSet<string> handledPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (StorageFile file in newFiles.Where(x => !x.IsInLocalFolder && x.File != null).Select(x => x.File))
                {
                    if (!handledPaths.Add(file.Path))
                    {
                        continue;
                    }

                    if (!tokenMapping.Remove(file.Path))
                    {
                        recentList.Add(file);
                    }
                }

                foreach (var token in tokenMapping.Values)
                {
                    recentList.Remove(token);
                }

                return Task.FromResult(true);
            });
        }
    }
}
