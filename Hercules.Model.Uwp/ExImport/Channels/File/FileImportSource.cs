// ==========================================================================
// FileImportSource.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using GP.Utils;

namespace Hercules.Model.ExImport.Channels.File
{
    public sealed class FileImportSource : IImportSource
    {
        public string NameKey
        {
            get { return "File"; }
        }

        public async Task<List<ImportResult>> ImportAsync(IImporter importer)
        {
            Guard.NotNull(importer, nameof(importer));

            var result = new List<ImportResult>();

            var filePicker = new FileOpenPicker();

            foreach (var extension in importer.Extensions)
            {
                filePicker.FileTypeFilter.Add(extension.Extension);
            }

            var file = await filePicker.PickSingleFileAsync();

            if (file == null)
            {
                return result;
            }

            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                var nameWithoutExtension = file.Name;

                var lastDot = file.Name.LastIndexOf('.');

                if (lastDot > 0)
                {
                    nameWithoutExtension = nameWithoutExtension.Substring(0, lastDot);
                }

                result = await importer.ImportAsync(fileStream, nameWithoutExtension);
            }

            return result;
        }
    }
}
