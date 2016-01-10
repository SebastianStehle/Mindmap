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
using Hercules.Model;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;

namespace Hercules.App.Components.Implementations
{
    public sealed class MindmapPrintService : IMindmapPrintService
    {
        private IPrintDocumentSource printDocument;

        public async Task PrintAsync(Document document, IRenderer renderer)
        {
            if (printDocument != null)
            {
                IDisposable disposable = printDocument as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            if (document != null && renderer != null)
            {
                printDocument = renderer.Print();

                PrintManager printManager = PrintManager.GetForCurrentView();

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
            string heading = ResourceManager.GetString("Print_Heading");

            args.Request.CreatePrintTask(heading, a =>
            {
                a.SetSource(printDocument);
            });
        }
    }
}
