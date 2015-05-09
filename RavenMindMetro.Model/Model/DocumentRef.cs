// ==========================================================================
// DocumentRef.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// Represents a lightweight readonly reference to a document in the storage.
    /// </summary>
    public sealed class DocumentRef
    {
        #region Fields

        private readonly Guid id;
        private readonly string name;
        private readonly DateTimeOffset lastUpdate;

        #endregion

        #region Properties

        /// <summary>
        /// The identity to the document.
        /// </summary>
        /// <value>
        /// The identity to the document.
        /// </value>
        public Guid Id
        {
            get { return id; }
        }

        /// <summary>
        /// The name of the document.
        /// </summary>
        /// <value>
        /// The name of the document.
        /// </value>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The datetime that specifies when the document has been updated the last time.
        /// </summary>
        /// <value>
        /// The datetime that specifies when the document has been updated the last time.
        /// </value>
        public DateTimeOffset LastUpdate
        {
            get { return lastUpdate; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="DocumentRef"/> class with the id, the last update datetime and the image.
        /// </summary>
        /// <param name="id">The identity to the document.</param>
        /// <param name="name">The name of the document. Cannot be null or whitespace.</param>
        /// <param name="lastUpdate">The datetime that specifies when the document has been updated the last time.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is null or whitespace.</exception>
        public DocumentRef(Guid id, string name, DateTimeOffset lastUpdate)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Cannot be null or whitespace.", "name");
            }

            this.id = id;
            this.name = name;
            this.lastUpdate = lastUpdate;
        }

        #endregion
    }
}
