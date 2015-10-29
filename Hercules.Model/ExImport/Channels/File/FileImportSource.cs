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

        public async Task<List<KeyValuePair<string, Document>>> ImportAsync(IImporter importer)
        {
            Guard.NotNull(importer, nameof(importer));

            List<KeyValuePair<string, Document>> result = new List<KeyValuePair<string, Document>>();

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
                    result = await importer.ImportAsync(fileStream);
                }
            }

            return result;
        }
    }
}
