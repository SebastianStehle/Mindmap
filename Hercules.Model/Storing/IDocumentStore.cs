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
        Task<IList<DocumentRef>> LoadAllAsync();

        Task<Document> LoadAsync(DocumentRef documentRef);

        Task DeleteAsync(DocumentRef documentRef);

        Task RenameAsync(DocumentRef documentRef, string newName);

        Task StoreAsync(DocumentRef documentRef, Document document);

        Task<DocumentRef> CreateAsync(string name, Document document);
    }
}
