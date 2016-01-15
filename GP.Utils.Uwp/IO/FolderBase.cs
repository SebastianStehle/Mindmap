// ==========================================================================
// StorageFolderBase.cs
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
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace GP.Utils.IO
{
    internal abstract class FolderBase : IFolder
    {
        public abstract Task<StorageFolder> GetFolderAsync();

        public Task<IFile> CreateFileAsync(string desiredName, FileExtension extension)
        {
            Guard.ValidFileName(desiredName, nameof(desiredName));
            Guard.NotNull(extension, nameof(extension));

            return HandleFileAsync(null, async (folder, userFile) =>
            {
                StorageFile storageFile = await folder.CreateFileAsync(desiredName, CreationCollisionOption.GenerateUniqueName);

                return (IFile)new File(storageFile.Name, extension, DateTime.UtcNow);
            });
        }

        public Task<List<IFile>> GetFilesAsync(FileExtension extension)
        {
            Guard.NotNull(extension, nameof(extension));

            return HandleFileAsync(null, async (folder, userFile) =>
            {
                List<IFile> result = new List<IFile>();

                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync(extension);

                foreach (StorageFile file in files)
                {
                    BasicProperties properties = await file.GetBasicPropertiesAsync();

                    result.Add(new File(file.Name, extension, properties.DateModified));
                }

                return result.OrderByDescending(x => x.ModifiedUtc).ToList();
            });
        }

        public Task CopyFilesAsync(IFolder source, FileExtension extension)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(extension, nameof(extension));

            return HandleFileAsync(null, async (folder, userFile) =>
            {
                FolderBase userFolder = (FolderBase)source;

                StorageFolder sourceFolder = await userFolder.GetFolderAsync();

                IReadOnlyList<StorageFile> files = await sourceFolder.GetFilesAsync(extension);

                foreach (StorageFile file in files)
                {
                    await file.CopyAsync(folder, file.Name, NameCollisionOption.ReplaceExisting);
                }

                return true;
            });
        }

        public Task<byte[]> OpenAsync(IFile file)
        {
            return HandleFileAsync(file, async (folder, userFile) =>
            {
                StorageFile storageFile;
                try
                {
                    storageFile = await folder.GetFileAsync(userFile.FullName);
                }
                catch
                {
                    throw new FileNotFoundException($"Cannot find the file '{userFile.FullName}'");
                }

                MemoryStream buffer;

                using (IRandomAccessStreamWithContentType stream = await storageFile.OpenReadAsync())
                {
                    buffer = await stream.ToMemoryStreamAsync();
                }

                return buffer.ToArray();
            });
        }

        public Task SaveAsync(IFile file, byte[] contents)
        {
            Guard.NotNull(contents, nameof(contents));

            return HandleFileAsync(file, async (folder, userFile) =>
            {
                StorageFile storageFile = await folder.GetOrCreateFileAsync(userFile.FullName);

                using (StorageStreamTransaction transaction = await storageFile.OpenTransactedWriteAsync())
                {
                    using (Stream stream = transaction.Stream.AsStreamForWrite())
                    {
                        await stream.WriteAsync(contents, 0, contents.Length);
                    }

                    await transaction.CommitAsync();
                }

                return userFile.Updated();
            });
        }

        public Task RenameAsync(IFile file, string desiredName)
        {
            Guard.ValidFileName(desiredName, nameof(desiredName));

            return HandleFileAsync(file, async (folder, userFile) =>
            {
                StorageFile storageFile;
                try
                {
                    storageFile = await folder.GetFileAsync(userFile.FullName);
                }
                catch
                {
                    throw new FileNotFoundException($"Cannot find the file '{userFile.FullName}'");
                }

                await storageFile.RenameAsync(desiredName, NameCollisionOption.GenerateUniqueName);

                return userFile.Updated().Rename(storageFile.Name);
            });
        }

        public Task<bool> DeleteAsync(IFile file)
        {
            return HandleFileAsync(file, async (folder, userFile) =>
            {
                bool isDeleted = await folder.TryDeleteIfExistsAsync(userFile.FullName);

                if (isDeleted)
                {
                    userFile.Updated().Deleted();
                }

                return isDeleted;
            });
        }

        private async Task<T> HandleFileAsync<T>(IFile file, Func<StorageFolder, File, Task<T>> handler)
        {
            Guard.NotNull(file, nameof(file));

            File userFile = null;

            if (file != null)
            {
                userFile = file as File;

                if (userFile == null)
                {
                    throw new ArgumentException("File is not a valid type.", nameof(file));
                }
            }

            StorageFolder folder = await GetFolderAsync();

            return await handler(folder, userFile);
        }
    }
}
