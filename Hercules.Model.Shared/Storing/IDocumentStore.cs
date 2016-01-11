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
        Task<List<DocumentRef>> LoadAllAsync();

        Task RenameAsync(DocumentRef documentRef, string newName);

        Task StoreAsync(DocumentRef documentRef, Document document);

        Task<bool> DeleteAsync(DocumentRef documentRef);

        Task<Document> LoadAsync(DocumentRef documentRef);

        Task<DocumentRef> CreateAsync(string name, Document document);
    }
}
