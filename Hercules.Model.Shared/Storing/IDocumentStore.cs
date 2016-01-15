// ==========================================================================
// IDocumentStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hercules.Model.Storing
{
    public interface IDocumentStore
    {
        Task<List<IDocumentRef>> LoadAllAsync();

        Task RenameAsync(IDocumentRef documentRef, string newName);

        Task StoreAsync(IDocumentRef documentRef, Document document);

        Task<bool> DeleteAsync(IDocumentRef documentRef);

        Task<Document> LoadAsync(IDocumentRef documentRef);

        Task<IDocumentRef> CreateAsync(string name, Document document);
    }
}
