// ==========================================================================
// FileExportTarget.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Linq;
using System.Threading.Tasks;
using GP.Windows;
using Hercules.Model;
using Hercules.Model.Export;
using Hercules.Model.Rendering;

namespace Hercules.App.Components.Implementations
{
    public sealed class FileExportTarget : IExportTarget
    {
        private readonly IMessageDialogService dialogService;

        public string NameKey
        {
            get { return "File"; }
        }

        public FileExportTarget(IMessageDialogService dialogService)
        {
            Guard.NotNull(dialogService, nameof(dialogService));

            this.dialogService = dialogService;
        }

        public Task ExportAsync(Document document, IExporter exporter, IRenderer renderer)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNull(exporter, nameof(exporter));

            return dialogService.SaveFileDialogAsync(exporter.Extensions.Select(x => x.Extension).ToArray(), s => exporter.ExportAsync(document, renderer, s));
        }
    }
}
