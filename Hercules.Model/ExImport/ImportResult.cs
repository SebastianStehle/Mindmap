// ==========================================================================
// ImportResult.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows;

namespace Hercules.Model.ExImport
{
    public sealed class ImportResult
    {
        private readonly string name;
        private readonly Document document;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Document Document
        {
            get
            {
                return document;
            }
        }

        public ImportResult(Document document, string name)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNullOrEmpty(name, nameof(name));

            this.document = document;

            this.name = name;
        }
    }
}
