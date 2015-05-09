// ==========================================================================
// Document.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RavenMind.Model
{
    /// <summary>
    /// Provides loading and storing functionalities for documents.
    /// </summary>
    public interface IDocumentStore
    {
        /// <summary>
        /// Loads all documents.
        /// </summary>
        /// <returns>
        /// The list of documents from the store.
        /// </returns>
        Task<IList<DocumentRef>> LoadAllAsync();

        /// <summary>
        /// Loads the document for the specified document info reference.
        /// </summary>
        /// <param name="documentId">The reference to the document. Cannot be null.</param>
        /// <returns>
        /// The loaded document.
        /// </returns>
        Task<Document> LoadAsync(Guid documentId);

        /// <summary>
        /// Deletes the specified document.
        /// </summary>
        /// <param name="documentId">The document to delete. Cannot be null.</param>n>
        /// <returns>
        /// The task object that can be used to wait for the async operation.
        /// </returns>
        Task DeleteAsync(Guid documentId);

        /// <summary>
        /// Saves the specified document.
        /// </summary>
        /// <param name="document">The document to save. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is null.</exception>
        /// <returns>
        /// The task object that can be used to wait for the async operation.
        /// </returns>
        Task<DocumentRef> StoreAsync(Document document);
    }
}
