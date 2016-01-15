// ==========================================================================
// JsonDocumentRef.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Utils.IO;

namespace Hercules.Model.Storing.Json
{
    internal sealed class JsonDocumentRef : IDocumentRef
    {
        private readonly IFile userFile;

        public string DocumentName
        {
            get { return userFile.Name; }
        }

        public DateTimeOffset ModifiedUtc
        {
            get { return userFile.ModifiedUtc; }
        }

        public IFile UserFile
        {
            get { return userFile; }
        }

        public JsonDocumentRef(IFile userFile)
        {
            this.userFile = userFile;
        }
    }
}
