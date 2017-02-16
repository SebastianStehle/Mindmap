// ==========================================================================
// MindappImporter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GP.Utils;
using Hercules.Model.Storing;
using Hercules.Model.Storing.Json;

namespace Hercules.Model.ExImport.Formats.Mindapp
{
   public sealed class MindappImporter : IImporter
    {
        public string NameKey
        {
            get { return "Mindapp"; }
        }

        public IEnumerable<FileExtension> Extensions
        {
            get { yield return Constants.FileExtension; }
        }

        public Task<List<ImportResult>> ImportAsync(Stream stream, string name)
        {
            Guard.NotNull(stream, nameof(stream));

            return Task.Run(() =>
            {
                var result = new List<ImportResult>();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var document = JsonDocumentSerializer.Deserialize(stream);

                    result.Add(new ImportResult(document, name));
                }

                return result;
            });
        }
    }
}
