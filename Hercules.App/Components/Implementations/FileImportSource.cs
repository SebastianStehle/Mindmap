// ==========================================================================
// FileImportSource.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GP.Windows;
using Hercules.Model;
using Hercules.Model.Export;

namespace Hercules.App.Components.Implementations
{
    public sealed class FileImportSource : IImportSource
    {
        private readonly IMessageDialogService dialogService;

        public string NameKey
        {
            get { return "File"; }
        }

        public FileImportSource(IMessageDialogService dialogService)
        {
            Guard.NotNull(dialogService, nameof(dialogService));

            this.dialogService = dialogService;
        }

        public async Task<List<KeyValuePair<string, Document>>> ImportAsync(IImporter importer)
        {
            Guard.NotNull(importer, nameof(importer));

            List<KeyValuePair<string, Document>> result = new List<KeyValuePair<string, Document>>();

            await dialogService.OpenFileDialogAsync(importer.Extensions.Select(x => x.Extension).ToArray(), async s =>
            {
                result = await importer.ImportAsync(s);
            });

            return result;
        }
    }
}
