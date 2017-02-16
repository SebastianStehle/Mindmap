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
using Windows.Storage.AccessCache;
using GP.Utils;
using Microsoft.HockeyApp;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Hercules.Model.Storing
{
    public sealed class DocumentFileRecentList
    {
        private readonly StorageItemMostRecentlyUsedList recentList = StorageApplicationPermissions.MostRecentlyUsedList;
        private readonly Dictionary<string, string> tokenMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<DocumentFile> files = new List<DocumentFile>();

        public async Task LoadAsync<T>(ICollection<T> list, Func<DocumentFile, T> factory)
        {
            Guard.NotNull(list, nameof(list));
            Guard.NotNull(factory, nameof(factory));

            var loadedFiles = await LoadAsync();

            loadedFiles.Foreach(x => list.Add(factory(x)));
        }

        public Task<IReadOnlyList<DocumentFile>> LoadAsync()
        {
            return FileQueue.EnqueueAsync<IReadOnlyList<DocumentFile>>(async () =>
            {
                try
                {
                    var unsortedFiles =
                        new Dictionary<string, DocumentFile>(StringComparer.OrdinalIgnoreCase);

                    await LoadFilesFromLocalStoreAsync(unsortedFiles);
                    await LoadFilesFromRecentListAsync(unsortedFiles);

                    files.Clear();

                    foreach (var sortedFile in unsortedFiles.Values.OrderByDescending(x => x.ModifiedUtc))
                    {
                        files.Add(sortedFile);
                    }

                    return files;
                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies("LoadRecentList"));
                    throw;
                }
            });
        }

        private static async Task LoadFilesFromLocalStoreAsync(IDictionary<string, DocumentFile> unsortedFiles)
        {
            foreach (var file in await LocalStore.GetFilesQueuedAsync())
            {
                if (!unsortedFiles.ContainsKey(file.Path) && file.FileType == ".mmd")
                {
                    unsortedFiles.Add(file.Path, await DocumentFile.OpenAsync(file, true));
                }
            }
        }

        private async Task LoadFilesFromRecentListAsync(IDictionary<string, DocumentFile> unsortedFiles)
        {
            if (!PlattformDetector.IsDesktop)
            {
                return;
            }

            tokenMapping.Clear();

            var filesHandled = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in recentList.Entries.ToList())
            {
                try
                {
                    var file = await recentList.GetFileAsync(entry.Token);

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
                        unsortedFiles.Add(file.Path, await DocumentFile.OpenAsync(file, false));
                    }
                }
                catch (FileNotFoundException)
                {
                    recentList.Remove(entry.Token);
                }
            }
        }

        public Task<bool> SaveAsync(IEnumerable<DocumentFile> newFiles)
        {
            Guard.NotNull(newFiles, nameof(newFiles));

            if (!PlattformDetector.IsDesktop)
            {
                return Task.FromResult(true);
            }

            return FileQueue.EnqueueAsync(() =>
            {
                try
                {
                    var errors = 0;

                    var handledPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (
                        var file in
                            newFiles.Where(x => !x.IsInLocalFolder && x.File != null).Select(x => x.File))
                    {
                        try
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
                        catch
                        {
                            errors++;
                        }
                    }

                    foreach (var token in tokenMapping.Values)
                    {
                        try
                        {
                            recentList.Remove(token);
                        }
                        catch
                        {
                            errors++;
                        }
                    }

                    return Task.FromResult(errors == 0);
                }
                catch (Exception e)
                {
                    HockeyClient.Current.TrackException(e, GetExceptionProperies("SaveRecentList"));
                    throw;
                }
            });
        }

        private static IDictionary<string, string> GetExceptionProperies(string action)
        {
            return new Dictionary<string, string>
            {
                { "FileAction", action }
            };
        }
    }
}
