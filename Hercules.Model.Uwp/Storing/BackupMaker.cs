// ==========================================================================
// BackupMaker.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Hercules.Model.Storing.Json;

namespace Hercules.Model.Storing
{
    public static class BackupMaker
    {
        public static async Task MakeBackupAsync(IEnumerable<DocumentFile> files)
        {
            var backupFile = await LocalStore.CreateOrOpenFileQueuedAsync("Backup.zip");

            var histories = new Dictionary<string, JsonHistory>();

            foreach (var file in files.Where(x => x.Document != null))
            {
                var name = file.Name + ".mmd";

                if (file.Path != null)
                {
                    name = file.Path;
                    name = name.Replace('/', '_');
                    name = name.Replace(':', '_');
                    name = name.Replace('\\', '_');
                }

                histories.Add(name, new JsonHistory(file.Document));
            }

            await FileQueue.EnqueueAsync(async () =>
            {
                using (var transaction = await backupFile.OpenTransactedWriteAsync())
                {
                    using (var archive = new ZipArchive(transaction.Stream.AsStream(), ZipArchiveMode.Update))
                    {
                        foreach (var kvp in histories)
                        {
                            var entry = archive.GetEntry(kvp.Key) ?? archive.CreateEntry(kvp.Key);

                            using (var entrySteam = entry.Open())
                            {
                                JsonDocumentSerializer.Serialize(kvp.Value, entrySteam);
                            }
                        }
                    }

                    await transaction.CommitAsync();
                }
            });
        }
    }
}
