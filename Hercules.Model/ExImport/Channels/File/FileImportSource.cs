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
using Windows.Storage;
using Windows.Storage.Pickers;
using GP.Windows;

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

            List<ImportResult> result = new List<ImportResult>();

            FileOpenPicker filePicker = new FileOpenPicker();

            foreach (FileExtension extension in importer.Extensions)
            {
                filePicker.FileTypeFilter.Add(extension.Extension);
            }

            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                using (Stream fileStream = await file.OpenStreamForReadAsync())
                {
                    string nameWithoutExtension = file.Name;

                    int lastDot = file.Name.LastIndexOf('.');

                    if (lastDot > 0)
                    {
                        nameWithoutExtension = nameWithoutExtension.Substring(0, lastDot);
                    }

                    result = await importer.ImportAsync(fileStream, nameWithoutExtension);
                }
            }

            return result;
        }
    }
}
