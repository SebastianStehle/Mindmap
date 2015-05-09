// ==========================================================================
// DocumentRef.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using SE.Metro;
using System;

namespace RavenMind.Model.Storing
{
    public sealed class DocumentRef
    {
        private readonly Guid id;
        private readonly string name;
        private readonly DateTimeOffset lastUpdate;
        
        public Guid Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public DateTimeOffset LastUpdate
        {
            get
            {
                return lastUpdate;
            }
        }

        public DocumentRef(Guid id, string name, DateTimeOffset lastUpdate)
        {
            Guard.NotNull(name, "name");

            this.id = id;
            this.name = name;
            this.lastUpdate = lastUpdate;
        }
    }
}
