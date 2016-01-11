// ==========================================================================
// DocumentObjectWithId.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public abstract class DocumentObjectWithId : DocumentObject
    {
        private readonly Guid id;

        public Guid Id
        {
            get { return id; }
        }

        protected DocumentObjectWithId(Guid id)
        {
            this.id = id;
        }
    }
}
