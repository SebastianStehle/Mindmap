// ==========================================================================
// JsonDocumentStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GP.Utils;
using GP.Utils.IO;
// ReSharper disable ConvertToLambdaExpression

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonDocumentStore : IDocumentStore
    {
        private const string DefaultSubfolder = "Mindapp";
        private readonly TaskFactory taskFactory;
        private readonly IFolder localFolder;

        public JsonDocumentStore(IFileSystem userStorage, TaskFactory taskFactory)
            : this(userStorage, taskFactory, DefaultSubfolder)
        {
        }

        private JsonDocumentStore(IFileSystem userStorage, TaskFactory taskFactory, string subfolderName)
        {
            Guard.NotNull(userStorage, nameof(userStorage));
            Guard.NotNull(taskFactory, nameof(taskFactory));
            Guard.NotNullOrEmpty(subfolderName, nameof(subfolderName));

            this.taskFactory = taskFactory;

            localFolder = userStorage.GetLocalFolder(subfolderName);
        }

        public Task<List<IDocumentRef>> LoadAllAsync()
        {
            return taskFactory.StartNew(async () =>
            {
                List<IFile> files = await localFolder.GetFilesAsync(JsonDocumentSerializer.FileExtension);

                return files.Select(x => (IDocumentRef)new JsonDocumentRef(x)).ToList();
            }).Unwrap();
        }

        public Task<Document> LoadAsync(IDocumentRef documentRef)
        {
            Guard.NotNull(documentRef, nameof(documentRef));

            JsonDocumentRef jsonRef = (JsonDocumentRef)documentRef;

            return taskFactory.StartNew(async () =>
            {
                byte[] contents = await localFolder.OpenAsync(jsonRef.UserFile);

                return JsonDocumentSerializer.Deserialize(contents);
            }).Unwrap();
        }

        public Task StoreAsync(IDocumentRef documentRef, Document document)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(documentRef, nameof(documentRef));

            JsonDocumentRef jsonRef = (JsonDocumentRef)documentRef;

            JsonHistory history = new JsonHistory(document);

            return taskFactory.StartNew(async () =>
            {
                byte[] contents = JsonDocumentSerializer.Serialize(history);

                await localFolder.SaveAsync(jsonRef.UserFile, contents);
            }).Unwrap();
        }

        public Task RenameAsync(IDocumentRef documentRef, string newName)
        {
            Guard.NotNull(documentRef, nameof(documentRef));
            Guard.ValidFileName(newName, nameof(newName));

            JsonDocumentRef jsonRef = (JsonDocumentRef)documentRef;

            return taskFactory.StartNew(async () =>
            {
                await localFolder.RenameAsync(jsonRef.UserFile, $"{newName}{JsonDocumentSerializer.FileExtension.Extension}");
            }).Unwrap();
        }

        public Task<bool> DeleteAsync(IDocumentRef documentRef)
        {
            Guard.NotNull(documentRef, nameof(documentRef));

            JsonDocumentRef jsonRef = (JsonDocumentRef)documentRef;

            return taskFactory.StartNew(async () =>
            {
                return await localFolder.DeleteAsync(jsonRef.UserFile);
            }).Unwrap();
        }

        public Task<IDocumentRef> CreateAsync(string name, Document document)
        {
            Guard.NotNull(document, nameof(document));
            Guard.ValidFileName(name, nameof(name));

            JsonHistory history = new JsonHistory(document);

            return taskFactory.StartNew(async () =>
            {
                IFile file = await localFolder.CreateFileAsync(name, JsonDocumentSerializer.FileExtension);

                byte[] contents = JsonDocumentSerializer.Serialize(history);

                await localFolder.SaveAsync(file, contents);

                return (IDocumentRef)new JsonDocumentRef(file);
            }).Unwrap();
        }
    }
}
