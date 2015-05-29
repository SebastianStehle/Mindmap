// ==========================================================================
// IDocumentStore.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindmapApp.Model.Storing
{
    public interface IDocumentStore
    {
        Task<IList<DocumentRef>> LoadAllAsync();

        Task<Document> LoadAsync(Guid documentId);

        Task DeleteAsync(Guid documentId);

        Task<DocumentRef> StoreAsync(Document document);
    }
}
