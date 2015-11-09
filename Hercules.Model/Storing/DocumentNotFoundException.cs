// ==========================================================================
// DocumentNotFoundException.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model.Storing
{
    public sealed class DocumentNotFoundException : Exception
    {
        public DocumentNotFoundException()
        {
        }

        public DocumentNotFoundException(string message)
            : base(message)
        {
        }

        public DocumentNotFoundException(string message, Exception inner) 
            : base(message, inner)
        {
        }
    }
}
