// ==========================================================================
// MindmapPrintService.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using GP.Utils;
using Hercules.Model;
using Hercules.Model.Rendering;

namespace Hercules.App.Components.Implementations
{
    public sealed class MindmapPrintService : IMindmapPrintService
    {
        private IPrintDocumentSource printDocument;

        public async Task PrintAsync(Document document, IPrintableRenderer renderer)
        {
            if (printDocument != null)
            {
                var disposable = printDocument as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            if (document != null && renderer != null)
            {
                printDocument = renderer.Print();

                var printManager = PrintManager.GetForCurrentView();

                printManager.PrintTaskRequested += PrintManager_PrintTaskRequested;
                try
                {
                    await PrintManager.ShowPrintUIAsync();
                }
                finally
                {
                    printManager.PrintTaskRequested -= PrintManager_PrintTaskRequested;
                }
            }
        }

        private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var heading = LocalizationManager.GetString("Print_Heading");

            args.Request.CreatePrintTask(heading, a =>
            {
                a.SetSource(printDocument);
            });
        }
    }
}
