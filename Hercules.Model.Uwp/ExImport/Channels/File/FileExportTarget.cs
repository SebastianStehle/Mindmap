﻿// ==========================================================================
// FileExportTarget.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using GP.Utils;
using Hercules.Model.Rendering;

namespace Hercules.Model.ExImport.Channels.File
{
    public sealed class FileExportTarget : IExportTarget
    {
        public string NameKey
        {
            get { return "File"; }
        }

        public async Task ExportAsync(string name, Document document, IExporter exporter, IRenderer renderer)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNull(exporter, nameof(exporter));

            var filePicker = new FileSavePicker();

            if (exporter.Extensions.Any())
            {
                filePicker.SuggestedFileName = name + exporter.Extensions.First().Extension;

                foreach (var extension in exporter.Extensions)
                {
                    filePicker.FileTypeChoices.Add(extension.Extension, new List<string> { extension.Extension });
                }
            }

            var file = await filePicker.PickSaveFileAsync();

            if (file != null)
            {
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    await exporter.ExportAsync(document, renderer, fileStream);
                }
            }
        }
    }
}
